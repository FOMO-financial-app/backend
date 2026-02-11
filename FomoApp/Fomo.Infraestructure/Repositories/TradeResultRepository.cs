using Fomo.Application.DTO.TradeResult;
using Fomo.Domain.Entities;
using Fomo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fomo.Infrastructure.Repositories
{
    public class TradeResultRepository : ITradeResultRepository
    {
        private readonly EFCoreDbContext _dbContext;

        public TradeResultRepository(EFCoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TradeResult?> GetByIdAsync(int id)
        {
            return await _dbContext.TradeResults
                .Include(tr => tr.TradeMethod)
                .FirstOrDefaultAsync(tr => tr.TradeResultId == id);
        }

        public async Task<List<TradeResultDTO>> GetPaginatedAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var resultsList = await _dbContext.TradeResults
                .AsNoTracking()
                .OrderByDescending(tr => tr.EntryDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(tr => new TradeResultDTO
                {
                    TradeResultId = tr.TradeResultId,
                    Symbol = tr.Symbol,
                    EntryPrice = tr.EntryPrice,
                    ExitPrice = tr.ExitPrice,
                    Profit = tr.Profit,
                    NumberOfStocks = tr.NumberOfStocks,
                    EntryDate = tr.EntryDate,
                    ExitDate = tr.ExitDate,
                    TradeMethod = new TradeMethodDTO
                    {
                        Sma = tr.TradeMethod.Sma,
                        Bollinger = tr.TradeMethod.Bollinger,
                        Stochastic = tr.TradeMethod.Stochastic,
                        Rsi = tr.TradeMethod.Rsi,
                        Other = tr.TradeMethod.Other
                    },
                    UserName = tr.User.Name                    
                })
                .ToListAsync();

            return resultsList;
        }

        public async Task<List<TradeResultDTO>> GetPaginatedByUserAsync(int page, int pageSize, int user)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var resultsList = await _dbContext.TradeResults
                .AsNoTracking()
                .Where(tr => tr.UserId == user)
                .OrderByDescending(tr => tr.EntryDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(tr => new TradeResultDTO
                {
                    TradeResultId = tr.TradeResultId,
                    Symbol = tr.Symbol,
                    EntryPrice = tr.EntryPrice,
                    ExitPrice = tr.ExitPrice,
                    Profit = tr.Profit,
                    NumberOfStocks = tr.NumberOfStocks,
                    EntryDate = tr.EntryDate,
                    ExitDate = tr.ExitDate,
                    TradeMethod = new TradeMethodDTO
                    {
                        Sma = tr.TradeMethod.Sma,
                        Bollinger = tr.TradeMethod.Bollinger,
                        Stochastic = tr.TradeMethod.Stochastic,
                        Rsi = tr.TradeMethod.Rsi,
                        Other = tr.TradeMethod.Other
                    },
                    UserName = tr.User.Name
                })
                .ToListAsync();

            return resultsList;
        }

        public async Task<int> CountRecordsAsync()
        {
            var numberOfRecords = await _dbContext.TradeResults
                .CountAsync();

            return numberOfRecords;
        }

        public async Task<int> CountRecordsByUserAsync(int user)
        {
            var numberOfRecords = await _dbContext.TradeResults
                .Where(tr => tr.UserId == user)
                .CountAsync();

            return numberOfRecords;
        }

        public async Task InsertAsync(TradeResult tradeResult)
        {
            await _dbContext.TradeResults.AddAsync(tradeResult);             
        }


        public async Task<bool> DeleteIfExistsAsync(int id)
        {
            var result = await _dbContext.TradeResults.FirstOrDefaultAsync(tr => tr.TradeResultId == id);

            if (result == null)
            {
                return false;
            }

            _dbContext.TradeResults.Remove(result);
            return true;
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
