using Fomo.Infrastructure.Repositories;
using System.Security.Claims;

namespace Fomo.Api.Helpers
{
    public class UserValidateHelper : IUserValidateHelper
    {
        private readonly IUserRepository _userRepository;

        public UserValidateHelper(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int?> GetUserIdAsync(ClaimsPrincipal user)
        {
            var auth0Id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value;

            if (auth0Id == null)
            {
                return null;
            }

            return await _userRepository.GetUserIdByAuth0IdAsync(auth0Id);
        }
    }
}
