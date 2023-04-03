using Enterprise_Web.ViewModels;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterprise_Web.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(List<EmailAddress> emailAddresses, string subject, string htmlMessage);
    }
}
