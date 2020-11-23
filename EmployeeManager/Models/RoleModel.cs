using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Models
{
    public class RoleModel
    {
        [Required]
        public RoleList Name { get; set; }
        [BindProperty]
        public List<string> AllUserIds { get; set; }
    }
}
