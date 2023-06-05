using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Demo.Business.Interface;

namespace Demo.Business.Repository
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string message, string subject)
        {
            
            var client = new SmtpClient("172.16.10.7", 25)
            {
                EnableSsl = false,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("uttam.sojitra@internal.mail", "tatva123")
            };

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
