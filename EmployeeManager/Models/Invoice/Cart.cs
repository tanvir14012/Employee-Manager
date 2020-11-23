using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Models.Invoice
{
    public class Cart
    {
        public int Id { get; set; }
        public IList<CartItem> CartItems { get; set; }
        public double ItemsTotalPrice { get; set; }
        public double Shipping { get; set; }
        public double Vat { get; set; }
        [Display(Name = "Coupon Discount")]
        public double CouponDiscount { get; set; }
        [Display(Name = "Coupon Code")]
        public string CouponCode { get; set; }
        [Display(Name = "Grand Total")]
        public double GrandTotalPayable { get; set; }
    }
}
