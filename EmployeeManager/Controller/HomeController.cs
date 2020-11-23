using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Rotativa.AspNetCore;
using EmployeeManager.Models;
using EmployeeManager.Models.Invoice;
using EmployeeManager.Services;
using Wkhtmltopdf.NetCore;

namespace EmployeeManager.Controller
{
    [Route("[controller]/[action]")]
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IEmployeeCrud _employeeCrud;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IGeneratePdf _generatePdf;
        private readonly IEmailSender emailSender;
        private readonly IRenderViewService razorViewToString;

        public HomeController(IEmployeeCrud employeeCrud, IHostingEnvironment hostingEnvironment
            , IGeneratePdf generatePdf, IEmailSender emailSender, IRenderViewService razorViewToString)
        {
            _employeeCrud = employeeCrud;
            _hostingEnvironment = hostingEnvironment;
            _generatePdf = generatePdf;
            this.emailSender = emailSender;
            this.razorViewToString = razorViewToString;
        }
        // GET: /<controller>/
        [Route("~/")]
        public ViewResult Index()
        {
            var model = _employeeCrud.GetAll();
            return View(model);
        }
        [Route("{id?}")]
        public ViewResult Details(int? id)
        {
            var model = _employeeCrud.GetEmployee(id ?? 1);
            ViewBag.PgTitle = "Employee Model Info";
            return View(model);
        }

        [HttpGet]
        public ViewResult Create()
        {
            ViewBag.PgTitle = "Create Employee Form";
            return View();
        }

        [HttpPost]
        public IActionResult Create(EmpCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string fileName = null;
            if (model.Photo != null)
            {
                fileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                System.Diagnostics.Debug.WriteLine(fileName);
                try
                {
                    string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);
                    //System.Diagnostics.Debug.WriteLine(filePath);
                    model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

            var employee = new Employee
            {
                Name = model.Name,
                Email = model.Email,
                Department = model.Department,
                PhotoPath = fileName
            };

            var empModel = _employeeCrud.Add(employee);
            return RedirectToAction("Details", new { Id = empModel.Id });
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var employee = _employeeCrud.GetEmployee(id);
            var employeeUpdateViewModel = new EmployeeUpdateViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                PhotoPath = employee.PhotoPath
            };
            return View(employeeUpdateViewModel);
        }

        [HttpPost]
        public IActionResult Update(EmployeeUpdateViewModel employee)
        {
            if (!ModelState.IsValid)
            {
                return View(employee);
            }
            //Delete existing photo if a new photo is uploaded
            if (employee.Photo != null)
            {
                try
                {
                    //delete old image if it exists
                    if (employee.PhotoPath != null)
                    {
                        string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", employee.PhotoPath);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    //save new image
                    var fileName = Guid.NewGuid().ToString() + "_" + employee.Photo.FileName;
                    var newFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);
                    employee.Photo.CopyTo(new FileStream(newFilePath, FileMode.Create));
                    employee.PhotoPath = fileName;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
            _employeeCrud.Update(employee);
            return RedirectToAction("Details", new { Id = employee.Id });
        }

        public IActionResult Delete(int Id)
        {
            if (ModelState.IsValid)
            {
                _employeeCrud.Delete(Id);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GenerateInvoice()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GenerateInvoice(GenerateInvoiceView model)
        {
            //if(ModelState.IsValid)
            {
                //Save image and store path
                if (model.CompanyLogo != null)
                {
                    try
                    {
                        string fileName = "company_logo" + model.CompanyLogo.FileName.Split(".").Last();
                        string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);
                        model.CompanyLogo.CopyTo(new FileStream(filePath, FileMode.Create));
                        model.Vendor.BrandPhotoPath = filePath;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }

                }
                //populate Ids
                var rand = new Random();
                model.Id = rand.Next(1, 1000000);
                model.Customer.Id = rand.Next(1, 1000000);
                model.Cart.Id = rand.Next(1, 1000000);
                //populate amount
                var amount = 0.0;
                foreach (var item in model.Cart.CartItems)
                {
                    item.ItemId = rand.Next(1, 1000000);
                    amount += (item.Price ?? 0 - item.Discount ?? 0 + item.Vat ?? 0) * item.Quantity ?? 1;
                }
                model.Cart.ItemsTotalPrice = amount;
                model.Cart.Shipping = 0;
                model.Cart.CouponCode = "BOISHAKE";
                model.Cart.CouponDiscount = 50;
                model.Cart.GrandTotalPayable = amount - model.Cart.CouponDiscount;
                return ExportInvoice(model);
            }
            //return View(model);
        }
        public IActionResult Invoice()
        {
            var invoiceModel = new Invoice
            {
                Id = 16264,
                Vendor = new Vendor
                {
                    Name = "Cox Ltd",
                    BrandPhotoPath = "/images/brand.jpg",
                    WebAddress = "https://www.thecox.xyz",
                    Address = new Address
                    {
                        Id = 35,
                        ALineOne = "450/8 Cox Road, Moghbazar",
                        ALineTwo = "Ramna, Dhaka",
                        Country = "Bangladesh",
                        State = "Dhaka",
                        Zip = "1217",
                        Email = "enquery@thecox.xyz",
                        CellPhone = "+8801465231459"
                    }
                },
                Customer = new Customer
                {
                    Id = 14651,
                    BillingAddress = new Address
                    {
                        Id = 58,
                        ALineOne = "264/4 Green road, Nayatola",
                        ALineTwo = "Ramna, Dhaka 1217",
                        Country = "Bangladesh",
                        State = "Dhaka",
                        Zip = "1217",
                        Email = "raul45@gmail.com",
                        CellPhone = "+8801786531238"
                    },
                    HomeAddress = new Address
                    {
                        Id = 58,
                        ALineOne = "264/4 Green road, Nayatola",
                        ALineTwo = "Ramna, Dhaka 1217",
                        Country = "Bangladesh",
                        State = "Dhaka",
                        Zip = "1217",
                        Email = "raul45@gmail.com",
                        CellPhone = "+8801786531238"
                    },
                    FirstName = "Raul",
                    LastName = "Ibrahim"
                },
                Cart = new Cart
                {
                    Id = 3168,
                    CartItems = new List<CartItem>
                    {
                        new CartItem
                        {
                            ItemId = 26,
                            Name = "Amazon 50$ Gift Card",
                            Discount = 0.0,
                            Price = 4400.00,
                            Type = "Gift card",
                            Vat = 0.0
                        },

                        new CartItem
                        {
                            ItemId = 16,
                            Name = "Fifa 20 PSN Activation Key",
                            Discount = 0.0,
                            Price = 2200.00,
                            Type = "License Key",
                            Vat = 0.0
                        },

                        new CartItem
                        {
                            ItemId = 5,
                            Name = "Kaspersky Total Security 2020 - 1 Year 1 Pc ",
                            Discount = 0.0,
                            Price = 1700.00,
                            Type = "Giftcard",
                            Vat = 0.0
                        }
                    },
                    CouponCode = "BOISHAKH",
                    CouponDiscount = 100.00,
                    Vat = 0.0,
                    Shipping = 0.0,
                    ItemsTotalPrice = 4400.0 + 2200.0 + 1700.0,
                    GrandTotalPayable = 4400.0 + 2200.0 + 1700.0 - 100.0

                },

                paymentInfo = new PaymentInfo
                {
                    CartId = 3168,
                    CustomerId = 14651,
                    AmountPaid = 4400.0 + 2200.0 + 1700.0 - 100.0,
                    CardNo = "4321xxxxxxxx4256",
                    CardType = "VISA",
                    CardStatementShow = "Software shop",
                    GatewayCurrency = "BDT",
                    IpAddress = "119.325.213.112",
                    Trxtype = "E-commerce",
                    MerchantBankId = "36556 A",
                    BankApprovedCode = "sdaf346s3ad1f3a6sdf63",
                    MerchantBankUid = "631631sdfsd3f1sdf"
                }
            };
            return View(invoiceModel);
        }
        [HttpPost]
        public IActionResult ExportInvoice(GenerateInvoiceView model)
        {
            return new ViewAsPdf("ExportInvoice", model)
            {
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
            };
        }
        [HttpGet]
        public IActionResult EmailInvoice()
        {
            return View();
        }
            
        [HttpPost]
        public async Task<IActionResult> EmailInvoice(GenerateInvoiceView model, string toEmail)
        {
            if (model.CompanyLogo != null)
            {
                try
                {
                    string fileName = "company_logo" + model.CompanyLogo.FileName.Split(".").Last();
                    string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);
                    model.CompanyLogo.CopyTo(new FileStream(filePath, FileMode.Create));
                    model.Vendor.BrandPhotoPath = filePath;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }

            }
            //populate Ids
            var rand = new Random();
            model.Id = rand.Next(1, 1000000);
            model.Customer.Id = rand.Next(1, 1000000);
            model.Cart.Id = rand.Next(1, 1000000);
            //populate amount
            var amount = 0.0;
            foreach (var item in model.Cart.CartItems)
            {
                item.ItemId = rand.Next(1, 1000000);
                amount += (item.Price ?? 0 - item.Discount ?? 0 + item.Vat ?? 0) * item.Quantity ?? 1;
            }
            model.Cart.ItemsTotalPrice = amount;
            model.Cart.Shipping = 0;
            model.Cart.CouponCode = "BOISHAKE";
            model.Cart.CouponDiscount = 50;
            model.Cart.GrandTotalPayable = amount - model.Cart.CouponDiscount;

            var mailMessage = await razorViewToString.RazorViewToStringAsync("Home/ExportInvoice", model);
            var pdf = new ViewAsPdf("ExportInvoice", model)
            {
                FileName = "Invoice" + model.Id + ".pdf",
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
            };
            byte[] pdfContent = await pdf.BuildFile(ControllerContext);
            var body = new BodyBuilder();
            body.HtmlBody = mailMessage;
            body.Attachments.Add(pdf.FileName, pdfContent);

            await emailSender.SendMailAsync(toEmail, body, "Shopping Invoice");
            return RedirectToAction("Index");
        }
    }
}
