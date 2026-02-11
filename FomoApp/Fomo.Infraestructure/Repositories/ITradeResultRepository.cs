using Fomo.Application.DTO.TradeResult;
using Fomo.Domain.Entities;

namespace Fomo.Infrastructure.Repositories
{
    public interface ITradeResultRepository
    {
        Task<TradeResult?> GetByIdAsync(int id);
        Task<List<TradeResultDTO>> GetPaginatedAsync(int page, int pageSize);
        Task<int> CountRecordsAsync();
        Task InsertAsync(TradeResult tradeResult);
        Task<bool> DeleteIfExistsAsync(int id);
        Task SaveAsync();
    }
}
