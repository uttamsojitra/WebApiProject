using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Business.Interface
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string message, string subject);
    }
}
