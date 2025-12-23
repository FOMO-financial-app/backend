using Fomo.Domain.Entities;
using Fomo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fomo.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EFCoreDbContext _dbContext;

        public UserRepository (EFCoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetByAuth0IdAsync(string auth0id)
        {
            var user = await _dbContext.Users
                .Select(u => new User
                {
                    UserId = u.UserId,
                    Auth0Id = u.Auth0Id,
                    Name = u.Name,
                    Email = u.Email,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    SmaAlert = u.SmaAlert,
                    BollingerAlert = u.BollingerAlert,
                    StochasticAlert = u.StochasticAlert,
                    RsiAlert = u.RsiAlert,

                    TradeResults = u.TradeResults == null ? null : u.TradeResults.Select(tr => new TradeResult
                    {
                        TradeResultId = tr.TradeResultId,
                        Symbol = tr.Symbol,
                        EntryPrice = tr.EntryPrice,
                        ExitPrice = tr.ExitPrice,
                        Profit = tr.Profit,
                        NumberOfStocks = tr.NumberOfStocks,
                        EntryDate = tr.EntryDate,
                        ExitDate = tr.ExitDate,
                        TradeMethod = new TradeMethod
                        {
                            Sma = tr.TradeMethod.Sma,
                            Bollinger = tr.TradeMethod.Bollinger,
                            Stochastic = tr.TradeMethod.Stochastic,
                            Rsi = tr.TradeMethod.Rsi,
                            Other = tr.TradeMethod.Other
                        }
                    }).ToList()
                })
                .FirstOrDefaultAsync(u => u.Auth0Id == auth0id);

            return user;
        }

        public async Task<User?> GetOnlyUserByAuth0IdAsync(string auth0id)
        {
            var user = await _dbContext.Users
                .Select(u => new User
                {
                    UserId = u.UserId,
                    Auth0Id = u.Auth0Id,
                    Name = u.Name,
                    Email = u.Email,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    SmaAlert = u.SmaAlert,
                    BollingerAlert = u.BollingerAlert,
                    StochasticAlert = u.StochasticAlert,
                    RsiAlert = u.RsiAlert,
                })
                .FirstOrDefaultAsync(u => u.Auth0Id == auth0id);

            return user;
        }

        public async Task<User?> GetUserIdByAuth0IdAsync(string auth0id)
        {
            var user = await _dbContext.Users
                .Select(u => new User
                {
                    UserId = u.UserId,      
                    Auth0Id = u.Auth0Id,
                })
                .FirstOrDefaultAsync(u => u.Auth0Id == auth0id);

            return user;
        }

        public async Task<List<NameAndMail>> GetUsersByAlertAsync(AlertType alertType)
        {
            IQueryable<User> query = _dbContext.Users;

            switch (alertType)
            {
                case AlertType.Sma:
                    query = query.Where(u => u.SmaAlert);
                    break;
                case AlertType.Bollinger:
                    query = query.Where(u => u.BollingerAlert);
                    break;
                case AlertType.Stochastic:
                    query = query.Where(u => u.StochasticAlert);
                    break;
                case AlertType.Rsi:
                    query = query.Where(u => u.RsiAlert);
                    break;
            }

            return await query
                .Select(u => new NameAndMail
                {
                    Name = u.Name,
                    Email = u.Email
                })
                .ToListAsync();
        }


        public async Task InsertAsync(User user)
        {
            bool exist = await _dbContext.Users.AnyAsync(u => u.Auth0Id == user.Auth0Id);
            if (!exist)
            {
                await _dbContext.Users.AddAsync(user);
            }
        }

        public void UpdateAsync(User user)
        {
            _dbContext.Users.Update(user);
        }

        public async Task DeleteAsync(string auth0id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Auth0Id == auth0id);
            if (user != null)
            {
                _dbContext.Users.Remove(user);
            }
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
