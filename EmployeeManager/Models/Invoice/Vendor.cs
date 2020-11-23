using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Models.Invoice
{
    public class Vendor
    {
        public string Name { get; set; }
        [Display(Name="Website Address")]
        public string WebAddress { get; set; }
        public string BrandPhotoPath { get; set; }
        public Address Address { get; set; }
    }
}
