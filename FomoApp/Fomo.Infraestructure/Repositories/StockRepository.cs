using Fomo.Domain.Entities;
using Fomo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Fomo.Infrastructure.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly EFCoreDbContext _dbContext;

        public StockRepository (EFCoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Stock>> GetPaginatedStocks(int page, int pageSize)
        {
            if (page < 1) page = 1;

            var paginatedList = await _dbContext.Stocks
                .AsNoTracking()
                .OrderBy(s => s.Symbol)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return paginatedList;
        }

        public async Task<List<SymbolAndName>> GetFilteredStocks(string query)
        {
            var filteredList = await _dbContext.Stocks
                .Where(tr =>
                    tr.Name.StartsWith(query) ||
                    tr.Symbol.StartsWith(query))
                .Select(s => new SymbolAndName
                {
                    Symbol = s.Symbol,
                    Name = s.Name
                })
                .AsNoTracking()                
                .ToListAsync();

            return filteredList;
        }
        public async Task InsertListAsync(List<Stock> stocks)
        {
            if (stocks == null || stocks.Count == 0)
                return;

            await _dbContext.Stocks.AddRangeAsync(stocks);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CountRecordsAsync()
        {
            var numberOfRecords = await _dbContext.Stocks
                .CountAsync();

            return numberOfRecords;
        }

        public async Task TruncateAsync()
        {
            await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [Stocks]");
        }

        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return _dbContext.Database.BeginTransactionAsync();
        }
    }
}
