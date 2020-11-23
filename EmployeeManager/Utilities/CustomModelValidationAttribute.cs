using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Utilities
{
    public class CustomModelValidationAttribute: ValidationAttribute
    {
        private readonly string allowedDomain;

        public CustomModelValidationAttribute(string allowedDomain)
        {
            this.allowedDomain = allowedDomain;
        }
        public override bool IsValid(object value)
        {
            var domain = value.ToString().Split("@").ToArray().Last().ToLower();
            return domain.Equals(this.allowedDomain.ToLower());
        }
    }
}
