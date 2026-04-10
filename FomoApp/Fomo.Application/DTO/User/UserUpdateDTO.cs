using System.ComponentModel.DataAnnotations;

namespace Fomo.Application.DTO.User
{
    public class UserUpdateDTO
    {
        [MaxLength(50, ErrorMessage = "The symbol must not exceed 10 characters")]
        public string? Name { get; set; }
        public bool? SmaAlert { get; set; }
        public bool? BollingerAlert { get; set; }
        public bool? StochasticAlert { get; set; }
        public bool? RsiAlert { get; set; }
    }
}
