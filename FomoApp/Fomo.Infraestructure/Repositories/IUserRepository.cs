using Fomo.Domain.Entities;

namespace Fomo.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByAuth0IdAsync(string auth0id);
        Task<User?> GetOnlyUserByAuth0IdAsync(string auth0id);
        Task<int?> GetUserIdByAuth0IdAsync(string auth0id);
        Task<List<NameAndMail>> GetUsersByAlertAsync(AlertType alertType);
        Task<bool> InsertIfNotExistsAsync(User user);
        Task<bool> DeleteIfExistsAsync(string auth0id);
        Task SaveAsync();
    }
}
