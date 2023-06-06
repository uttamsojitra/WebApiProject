using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Demo.Business.Interface;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace Demo.Business.Repository
{
    public class EmailSender : IEmailSender
    {
        private readonly IWebHostEnvironment _env;

        //service to access hosting environment
        public EmailSender(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task SendEmailAsync(string email, string message, string subject)
        {
            
            var client = new SmtpClient("172.16.10.7", 25)
            {
                EnableSsl = false,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("uttam.sojitra@internal.mail", "tatva123")
            };

            if (_env.IsDevelopment())
            {
                email = "uttam.sojitra@internal.mail";
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress("uttam.sojitra@internal.mail"),
                To = { email },
                Subject = subject,
                IsBodyHtml = true,// Set the email body as HTML content
                Body = message
            };

            await client.SendMailAsync(mailMessage);
        }
    }
}
