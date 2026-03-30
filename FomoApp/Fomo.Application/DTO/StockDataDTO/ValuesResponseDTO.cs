using System.Text.Json.Serialization;

namespace Fomo.Application.DTO.StockDataDTO
{
    public record ValuesResponseDTO
    {
        [JsonPropertyName("meta")]
        public MetaDTO? MetaDTO { get; init; }

        [JsonPropertyName("values")]
        public List<ValuesDTO> Values { get; init; } = new List<ValuesDTO>();

        public bool IsRateLimited { get; set; } = false;

    }
}
