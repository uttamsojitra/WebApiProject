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
        public Task SendEmailAsync(string email, string message, string Subject)
        {
            var Email = email;
            var subject = Subject;
            var Message = message;

            var client = new SmtpClient("172.16.10.7", 25)
            {
                EnableSsl = false,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("uttam.sojitra@internal.mail", "tatva123")
            };

            return client.SendMailAsync(
                new MailMessage(from: "uttam.sojitra@internal.mail",
                                to: Email,
                                subject,
                                Message
                                ));
        }
    }
}
