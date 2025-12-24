namespace Fomo.Application.DTO.StockDataDTO
{
    public class ValuesAndChannelDTO
    {
        public MetaDTO? MetaDTO { get; init; }

        public List<ValuesDTO> Values { get; init; } = new List<ValuesDTO>();

        public List<decimal> Regression { get; set; } = new List<decimal>();

        public List<decimal> Upper { get; set; } = new List<decimal>();

        public List<decimal> Lower { get; set; } = new List<decimal>();
    }
}
