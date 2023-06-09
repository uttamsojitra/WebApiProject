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
using Demo.Entities.Model.ViewModel;
using Microsoft.Extensions.Options;

namespace Demo.Business.Repository
{
    public class EmailSender : IEmailSender
    {
        private readonly IWebHostEnvironment _env;
        private readonly IOptions<SmtpSettings> _smtpSetting;
       
        //service to access hosting environment
        public EmailSender(IWebHostEnvironment env, IOptions<SmtpSettings> smtp)
        {
            _env = env;
            _smtpSetting = smtp;        
        }
        public async Task SendEmailAsync(string email, string message, string subject)
        {

            var client = new SmtpClient(_smtpSetting.Value.Host, _smtpSetting.Value.Port)
            {
                EnableSsl = _smtpSetting.Value.EnableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpSetting.Value.Username, _smtpSetting.Value.Password)
            };

            if (_env.IsDevelopment())
            {
                email = _smtpSetting.Value.FromEmail;
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSetting.Value.FromEmail),
                To = { email },
                Subject = subject,
                IsBodyHtml = true, // Set the email body as HTML content
                Body = message
            };

            await client.SendMailAsync(mailMessage);
        }
    }
}
