using Fomo.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Fomo.Infrastructure.Repositories
{
    public interface IStockRepository
    {
        Task<List<Stock>> GetPaginatedStocks(int page, int pageSize);
        Task<List<SymbolAndName>> GetFilteredStocks(string query);
        Task InsertListAsync(List<Stock> stocks);
        Task<int> CountRecordsAsync();
        Task TruncateAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
