using Enterprise_Web.Repository.IRepository;
using Enterprise_Web.Services;
using Enterprise_Web.ViewModels;
using EnterpriseWeb.Data;
using SendGrid.Helpers.Mail;

namespace Enterprise_Web.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailSender _emailSender;
        public NotificationRepository(ApplicationDbContext dbContext, IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _emailSender = emailSender;
        }
        public async Task CheckAndSend(NotificationViewModel notificationViewModel)
        {
            if (notificationViewModel.IdeaId == null)
            {
                string subject = "NEW IDEA POSTED";
                string htmlMessage = $"{notificationViewModel.CreatedBy} have just contributed a new idea. Let's go check it out!";
                var qacEmail = _dbContext.Users.Where(q => q.Role == "QAC" && q.DepartmentId == notificationViewModel.DepartmentId).Select(q => q.Email).ToString();

                var emailAddresses = new List<EmailAddress>();
                var qacEmailAdd = new EmailAddress(qacEmail, "QA Coordinator");
                emailAddresses.Add(qacEmailAdd);

                await _emailSender.SendEmailAsync(emailAddresses, subject, htmlMessage);
            }
            else
            {
                string subject = "NEW COMMENT POSTED";
                string htmlMessage = $"{notificationViewModel.CreatedBy} have just commented on your idea. Let's go check it out!";
                var staffEmail = (from i in _dbContext.Ideas
                                  join c in _dbContext.Comments on i.Id equals c.IdeaId
                                  where c.IdeaId == notificationViewModel.IdeaId
                                  select i.CreatedBy).FirstOrDefault();

                var emailAddresses = new List<EmailAddress>();
                var staffEmailAdd = new EmailAddress(staffEmail, "Staff");
                emailAddresses.Add(staffEmailAdd);

                await _emailSender.SendEmailAsync(emailAddresses, subject, htmlMessage);
            }
        }
    }
}
