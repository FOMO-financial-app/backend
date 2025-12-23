namespace Fomo.Application.DTO.IndicatorsDTO
{
    public class BandsDTO
    {
        public List<decimal> UpperBand {  get; set; } = new List<decimal>();

        public List<decimal> LowerBand { get; set; } = new List<decimal>();

        public List<string> Date { get; set; } = new List<string>();
    }
}
