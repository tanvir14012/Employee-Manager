using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Services
{
    public interface IEmailSender
    {
          Task SendMailAsync(string toEmail, BodyBuilder bodyBuilder,
            string Subject, string EmailType="", string HTMLMessageContent = "");
    }
}
