using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace Fomo.Infrastructure.ExternalServices.StockService
{
    public class ExternalApiHelper : IExternalApiHelper
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly ILogger<ExternalApiHelper> _logger;

        public ExternalApiHelper (IOptions<ApiSettings> options, HttpClient httpClient, ILogger<ExternalApiHelper> logger) 
        {
            _httpClient = httpClient;
            _apiSettings = options.Value;
            _httpClient.BaseAddress = new Uri(_apiSettings.BaseUrl);
            _logger = logger;
        }

        public async Task<T?> GetAsync<T> (string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{endpoint}");

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    _logger.LogWarning("External API limit reached for endpoint: {Endpoint}", endpoint);
                    throw new ExternalApiLimitException();
                }

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (ExternalApiLimitException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling external API: {Endpoint}", endpoint);
                throw;
            }
        }
    }

    public class ExternalApiLimitException : Exception
    {
        public ExternalApiLimitException() : base("External API limit reached") { }
    }
}
