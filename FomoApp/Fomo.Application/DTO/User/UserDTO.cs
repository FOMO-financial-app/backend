using Fomo.Application.DTO.TradeResult;

namespace Fomo.Application.DTO.User
{
    public class UserDTO
    {
        public string? Name { get; set; }
        public bool? SmaAlert { get; set; }
        public bool? BollingerAlert { get; set; }
        public bool? StochasticAlert { get; set; }
        public bool? RsiAlert { get; set; }
    }
}
