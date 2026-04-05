namespace Fomo.Infrastructure.ExternalServices.MailService
{
    public interface IEmailService
    { 
        Task SendAsync(string to, string subject, string body);
    }
}
