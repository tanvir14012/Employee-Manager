using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Models.DTO.Invoice
{
    public class CartItem
    {
        public int ItemId { get; set; }
        [Display(Name = "Item Name")]
        public string Name { get; set; }
        public string Type { get; set; }
        public int? Quantity { get; set; }
        [Display(Name = "Price")]
        public double? Price { get; set; }
        public double? Discount { get; set; }
        public double? Vat { get; set; }
    }
}
