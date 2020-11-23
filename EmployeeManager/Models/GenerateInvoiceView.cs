using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Models
{
    public class GenerateInvoiceView: EmployeeManager.Models.Invoice.Invoice
    {
        [Display(Name = "Company Logo")]
        public IFormFile CompanyLogo { get; set; }
    }
}
