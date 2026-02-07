using Fomo.Domain.Entities;

namespace Fomo.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByAuth0IdAsync(string auth0id);
        Task<User?> GetOnlyUserByAuth0IdAsync(string auth0id);
        Task<int?> GetUserIdByAuth0IdAsync(string auth0id);
        Task<List<NameAndMail>> GetUsersByAlertAsync(AlertType alertType);
        Task InsertAsync(User user);
        Task DeleteAsync(string auth0id);
        Task SaveAsync();
    }
}
