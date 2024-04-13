using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace ecomm_project_clothes.Utility
{
    public class emailSender : IEmailSender
    {
        private EmailSettings _emailsettings {  get; }
        public emailSender(IOptions<EmailSettings> emailsettings)
        {
            _emailsettings = emailsettings.Value;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
           Execute(email,subject, htmlMessage).Wait();
            return Task.FromResult(0);
        }
        public async Task Execute(string email,string subject,string message)
        {
            try
            {
                string toEmail = string.IsNullOrEmpty(email) ?
                    _emailsettings.ToEmail : email;
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(_emailsettings.UsernameEmail, "My email name"),
                    
                };
                mail.To.Add(toEmail);
                mail.CC.Add(_emailsettings.CcEmail);
                mail.Subject="Shopping App:" + subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;
                using(SmtpClient smtp=new SmtpClient(_emailsettings.PrimaryDomain,_emailsettings.primaryPort))
                {
                    smtp.Credentials = new NetworkCredential(_emailsettings.UsernameEmail, _emailsettings.UsernamePassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch(Exception ex)
            {
                string str=ex.Message;
            }
        }
    }
}
