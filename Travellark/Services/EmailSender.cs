using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Travellark.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var emailSettings = _config.GetSection("EmailSettings");

            var client = new SmtpClient(emailSettings["Host"], int.Parse(emailSettings["Port"]))
            {
                Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]),
                EnableSsl = true
            };

            return client.SendMailAsync(
                new MailMessage(emailSettings["Username"], email, subject, message)
                {
                    IsBodyHtml = true
                });
        }
    }
}
