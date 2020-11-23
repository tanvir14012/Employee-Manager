using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Utils;
using Rotativa.AspNetCore;
using EmployeeManager.Controller;
using EmployeeManager.Models;
using EmployeeManager.Models.DTO.Invoice;
using EmployeeManager.Models.Entity;

namespace EmployeeManager.Services
{
    public class EmailService : IEmailSender
    {
        private readonly IOptions<EmailSetting> emailSetting;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IServiceProvider serviceProvider;

        public EmailService(IOptions<EmailSetting> emailSetting, IHostingEnvironment hostingEnvironment,
            IServiceProvider serviceProvider)
        {
            this.emailSetting = emailSetting;
            this.hostingEnvironment = hostingEnvironment;
            this.serviceProvider = serviceProvider;
        }
        public async Task SendMailAsync(string toEmail, BodyBuilder bodyBuilder, string subject,
            string emailType = "", string HTMLMessageContent = "")
        {
                var from = new MailboxAddress(emailSetting.Value.SenderName, emailSetting.Value.SenderEmail);
                var to = new MailboxAddress(toEmail);
                var mimemessage = new MimeMessage();
                mimemessage.From.Add(from);
                mimemessage.To.Add(to);
                mimemessage.Body = bodyBuilder.ToMessageBody();
                mimemessage.Subject = subject;

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(emailSetting.Value.MailServer, emailSetting.Value.MailPort, emailSetting.Value.EnableSsl);
                    await client.AuthenticateAsync(emailSetting.Value.SenderEmail, emailSetting.Value.SenderPassword);
                    await client.SendAsync(mimemessage);
                    await client.DisconnectAsync(true);
                }
                await Task.CompletedTask;
        }
    }
}
