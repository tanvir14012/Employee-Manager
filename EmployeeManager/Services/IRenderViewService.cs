using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Services
{
    public interface IRenderViewService
    {
        Task<string> RazorViewToStringAsync(string viewName, object model);
    }
}
