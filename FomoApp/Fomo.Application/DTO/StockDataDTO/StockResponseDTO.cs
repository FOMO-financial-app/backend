using System.Text.Json.Serialization;

namespace Fomo.Application.DTO.StockDataDTO
{
    public record StockResponseDTO
    {
        [JsonPropertyName("data")]
        public List<StockDTO> Data { get; init; } = new List<StockDTO>();

        public bool IsRateLimited { get; set; } = false;
    }
}
