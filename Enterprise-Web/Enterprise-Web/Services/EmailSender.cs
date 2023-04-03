using SendGrid;
using SendGrid.Helpers.Mail;
using System.Diagnostics;

namespace Enterprise_Web.Services
{
    public class EmailSender : IEmailSender
    {
        public string SendGridSecret { get; set; }

        public EmailSender(IConfiguration _config)
        {
            SendGridSecret = _config.GetValue<string>("SendGrid:SecretKey");
        }

        public Task SendEmailAsync(List<EmailAddress> emailAddresses, string subject, string htmlMessage)
        {
            var client = new SendGridClient(SendGridSecret);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("namntkgcd201381@fpt.edu.vn", "Idealli"),
                Subject = subject,
                HtmlContent = htmlMessage,
                Personalizations = new List<Personalization>
                {
                    new Personalization
                    {
                        Tos = emailAddresses
                    }
                }
            };
            var result = MailHelper.CreateSingleEmailToMultipleRecipients(msg.From, emailAddresses, msg.Subject, "", msg.HtmlContent);
            var res = client.SendEmailAsync(result);
            return Task.FromResult(res);
        }
    }
}
