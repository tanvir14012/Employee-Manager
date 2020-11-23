using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Utilities
{
    public class EmailTokenProvider<TUser>: DataProtectorTokenProvider<TUser> where TUser: class
    {
        public EmailTokenProvider(IDataProtectionProvider dataProtectionProvider, IOptions<EmailTokenProviderOptions> options) 
            : base(dataProtectionProvider, options)
        {

        }
    }
}
