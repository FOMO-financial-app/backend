using Fomo.Domain.Entities;

namespace Fomo.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByAuth0IdAsync(string auth0id);
        Task<User?> GetOnlyUserByAuth0IdAsync(string auth0id);
        Task<User?> GetUserIdByAuth0IdAsync(string auth0id);
        Task<List<NameAndMail>> GetUsersByAlertAsync(AlertType alertType);
        Task InsertAsync(User user);
        void UpdateAsync(User user);
        Task DeleteAsync(string auth0id);
        Task SaveAsync();
    }
}
