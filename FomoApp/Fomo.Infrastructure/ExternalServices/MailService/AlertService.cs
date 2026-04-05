using Fomo.Domain.Entities;
using Fomo.Infrastructure.ExternalServices.MailService.Alerts;

namespace Fomo.Infrastructure.ExternalServices.MailService
{
    public class AlertService : IAlertService
    {
        private readonly IGenericAlert _genericAlert;

        public AlertService (IGenericAlert genericAlert)
        {
            _genericAlert = genericAlert;
        }

        public async Task SendSmaAlert(List<decimal> closes, List<decimal> sma, string stock, string indicator)
        {
            if (closes[^1] > sma[^1] && closes[^2] <= sma[^2])
            {
                await _genericAlert.SendAlertAsync(AlertType.Sma, stock, indicator);
            }
        }

        public async Task SendBollingerAlert(List<decimal> closes, List<decimal> low, string stock, string indicator)
        {
            if (closes[^2] < low[^2] && closes[^1] > low[^1])
            {
                await _genericAlert.SendAlertAsync(AlertType.Bollinger, stock, indicator);
            }
        }

        public async Task SendStochasticAlert(List<decimal> kList, List<decimal> dList, string stock, string indicator)
        {
            if (kList[^1] > dList[^1] && kList[^2] <= dList[^2] && kList[^1] < 20 && kList[^1] > 10)
            {
                await _genericAlert.SendAlertAsync(AlertType.Stochastic, stock, indicator);
            }
        }

        public async Task SendRsiAlert(List<decimal> rsi, string stock, string indicator)
        {
            if (rsi[^1] < 30 && rsi[^1] > 20)
            {
                await _genericAlert.SendAlertAsync(AlertType.Rsi, stock, indicator);
            }
        }
    }
}
