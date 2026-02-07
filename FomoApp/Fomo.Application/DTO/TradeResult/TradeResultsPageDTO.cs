namespace Fomo.Application.DTO.TradeResult
{
    public class TradeResultsPageDTO
    {
        public List<TradeResultDTO> Data { get; set; } = new List<TradeResultDTO>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
