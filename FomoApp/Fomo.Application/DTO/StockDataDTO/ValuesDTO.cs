using System.Text.Json.Serialization;


namespace Fomo.Application.DTO.StockDataDTO
{
    public record ValuesDTO
    {
        [JsonPropertyName("datetime")]
        public string Date { get; init; } = string.Empty;

        [JsonPropertyName("open")]
        public string Open { get; init; } = string.Empty;

        [JsonPropertyName("high")]
        public string High { get; init; } = string.Empty;

        [JsonPropertyName("low")]
        public string Low { get; init; } = string.Empty;

        [JsonPropertyName("close")] 
        public string Close { get; init; } = string.Empty;

        [JsonPropertyName("volume")]
        public string Volume { get; init; } = string.Empty;
    }
}
