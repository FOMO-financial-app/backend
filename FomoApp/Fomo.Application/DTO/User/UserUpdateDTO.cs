namespace Fomo.Application.DTO.User
{
    public class UserUpdateDTO
    {
        public string? Name { get; set; }
        public bool? SmaAlert { get; set; }
        public bool? BollingerAlert { get; set; }
        public bool? StochasticAlert { get; set; }
        public bool? RsiAlert { get; set; }
    }
}
