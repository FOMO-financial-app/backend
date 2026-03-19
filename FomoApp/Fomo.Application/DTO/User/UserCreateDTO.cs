using System.ComponentModel.DataAnnotations;

namespace Fomo.Application.DTO.User
{
    public class UserCreateDTO
    {
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(320)]
        public string Email { get; set; } = string.Empty;
    }
}
