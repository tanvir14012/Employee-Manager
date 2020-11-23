using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Models
{
    public class EmpCreateViewModel
    {
        [Required]
        [RegularExpression("^[A-Za-z ]{2,50}$", ErrorMessage = "Name should contain only letters and length between 2 and 50")]
        public string Name { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z]+[a-zA-Z0-9_.]*@[a-zA-Z0-9_.]{3,10}$", ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        public Dept Department { get; set; }
        public IFormFile Photo { get; set; }
    }
}
