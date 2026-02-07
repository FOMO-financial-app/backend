using System.Security.Claims;

namespace Fomo.Api.Helpers
{
    public interface IUserValidateHelper
    {
        Task<int?> GetUserIdAsync(ClaimsPrincipal user);
    }
}
