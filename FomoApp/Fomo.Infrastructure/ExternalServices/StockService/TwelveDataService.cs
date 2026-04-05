using Fomo.Application.DTO.StockDataDTO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Fomo.Infrastructure.ExternalServices.StockService
{
    public class TwelveDataService : ITwelveDataService
    {
        private readonly IMemoryCache _cache;
        private readonly IExternalApiHelper _externalApiHelper;
        private readonly TwelveData _twelveData;

        public TwelveDataService(IMemoryCache cache, IOptions<TwelveData> options, IExternalApiHelper externalApiHelper)
        {
            _cache = cache;
            _externalApiHelper = externalApiHelper;
            _twelveData = options.Value;
        }

        public async Task<StockResponseDTO?> GetStocks()
        {
            try {
                string path = $"stocks?country=US&apikey={_twelveData.ApiKey}";

                return await _externalApiHelper.GetAsync<StockResponseDTO>(path);
            }
            catch (ExternalApiLimitException)
            {
                return new StockResponseDTO
                {
                    IsRateLimited = true
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ValuesResponseDTO?> GetTimeSeries(string symbol)
        {
            try
            {
                var cacheKey = $"timeseries_{symbol}";

                if (_cache.TryGetValue(cacheKey, out ValuesResponseDTO? cached))
                {
                    return cached;
                }

                string path = $"time_series?symbol={symbol}&interval=1day&outputsize=120&apikey={_twelveData.ApiKey}";

                var result = await _externalApiHelper.GetAsync<ValuesResponseDTO>(path);

                if (result == null)
                {
                    return null;
                }

                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                };

                _cache.Set(cacheKey, result, options);

                return result;
            }
            catch (ExternalApiLimitException)
            {
                return new ValuesResponseDTO
                {
                    IsRateLimited = true
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
