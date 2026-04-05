using Fomo.Domain.Entities;

namespace Fomo.Infrastructure.ExternalServices.MailService.Alerts
{
    public interface IGenericAlert
    {
        Task SendAlertAsync(AlertType alertType, string stock, string indicator);
    }
}
