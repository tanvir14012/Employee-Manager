using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Models.DTO
{
    public class RoleModel
    {
        public IList<UserInfo> UserList { get; set; }

        [Required]
        public RoleList Name { get; set; }
        [BindProperty]
        public List<string> AllUserIds { get; set; }

        public string OprMode { get; set; }
    }

    public class UserInfo
    {
        public string Uid { get; set; }
        public string Email { get; set; }
        public IList<string> RoleNames { get; set; }
    }
}
