namespace Fomo.Infrastructure.ExternalServices.MailService
{
    public interface IAlertService
    {
        Task SendSmaAlert(List<decimal> closes, List<decimal> sma, string stock, string indicator);
        Task SendBollingerAlert(List<decimal> closes, List<decimal> low, string stock, string indicator);
        Task SendStochasticAlert(List<decimal> kList, List<decimal> dList, string stock, string indicator);
        Task SendRsiAlert(List<decimal> rsi, string stock, string indicator);
    }
}
