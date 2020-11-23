using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManager.Controller
{
    public class ErrorController : Microsoft.AspNetCore.Mvc.Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HandleError(int statusCode)
        {
            switch(statusCode)
            {
                case 404: ViewBag.ErrorMessage = "The requested service is unavailable";
                    break;
                default: ViewBag.ErrorMessage = "Can not process the request";
                    break;
            }

            return View();
        }
    }
}