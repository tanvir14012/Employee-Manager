using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace EmployeeManager.Models.Entity
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        [RegularExpression("^[A-Za-z ]{2,50}$", ErrorMessage ="Name should contain only letters and length between 2 and 50")]
        public string Name { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z]+[a-zA-Z0-9_.]*@[a-zA-Z0-9_.]{3,10}$", ErrorMessage ="Invalid email address")]
        public string Email { get; set; }
        public Dept Department { get; set; }
        public string PhotoPath { get; set; }
    }
}
