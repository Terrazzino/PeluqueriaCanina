using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace PeluqueriaCanina.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        public SmtpEmailSender(IConfiguration config)
        {
            _config = config;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtpSettings = _config.GetSection("SmtpSettings");
            using (var smtp = new SmtpClient(smtpSettings["Server"], int.Parse(smtpSettings["Port"])))
            {
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(
                    smtpSettings["Username"],
                    smtpSettings["Password"]
                );

                var message = new MailMessage(
                    smtpSettings["SenderEmail"],
                    email,
                    subject,
                    htmlMessage
                );
                message.IsBodyHtml = true;

                await smtp.SendMailAsync(message);
            }
        }
    }
}

