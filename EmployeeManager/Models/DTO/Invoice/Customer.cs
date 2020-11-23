using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Models.DTO.Invoice
{
    public class Customer
    {
        public int Id { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Home Address")]
        public Address HomeAddress { get; set; }
        [Display(Name = "Billing Address")]
        public Address BillingAddress { get; set; }
    }
}
