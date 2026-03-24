using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Fomo.Infrastructure.ExternalServices.MailService
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var smtp = new SmtpClient($"{_settings.Client}")
            {
                Port = 587,
                Credentials = new NetworkCredential($"{_settings.Email}", $"{_settings.Token}"),
                EnableSsl = true
            };

            var mail = new MailMessage($"{_settings.Email}", to, subject, body)
            {
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(mail);
        }
    }
}
