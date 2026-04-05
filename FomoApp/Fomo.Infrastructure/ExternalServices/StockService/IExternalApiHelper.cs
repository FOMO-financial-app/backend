namespace Fomo.Infrastructure.ExternalServices.StockService
{
    public interface IExternalApiHelper
    {
        Task<T?> GetAsync<T>(string endpoint);
    }
}
