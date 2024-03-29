﻿using EmployeeManager.Models.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Models.DTO
{
    public class EmployeeUpdateViewModel: Employee
    {
        public IFormFile Photo { get; set; }
    }
}
