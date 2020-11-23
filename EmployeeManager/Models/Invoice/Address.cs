using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Models.Invoice
{
    public class Address
    {
        public int Id { get; set; }
        [Display(Name="Address Line 1")]
        public string ALineOne { get; set; }
        [Display(Name = "Address Line 2")]
        public string ALineTwo { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        [Display(Name = "Mobile No")]
        public string CellPhone { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
}
