namespace Fomo.Application.DTO.IndicatorsDTO
{
    public class StochasticDTO
    {
        public List<decimal> K { get; set; } = new List<decimal>();

        public List<string> Kdate { get; set; } = new List<string>();

        public List<decimal> D { get; set; } = new List<decimal>();

        public List<string> Ddate { get; set; } = new List<string>();
    }
}
