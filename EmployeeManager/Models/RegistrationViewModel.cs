using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManager.Utilities;

namespace EmployeeManager.Models
{
    public class RegistrationViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        //[Remote(action: "IsEmailExists", controller: "Account")]
        [CustomModelValidation(allowedDomain: "gmail.com", ErrorMessage ="Domain must be gmail.com")]
        public string User { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage ="Password and confirm password do not match")]
        public string ConfirmPassword { get; set; }
    }
}
