namespace Fomo.Application.DTO.IndicatorsDTO
{
    public class MainChannelDTO
    {
        public List<decimal> Regression {  get; set; } = new List<decimal>();

        public List<decimal> Upper { get; set;} = new List<decimal>();

        public List<decimal> Lower { get; set; } = new List<decimal>();
    }
}
