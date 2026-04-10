using System.ComponentModel.DataAnnotations;

namespace Fomo.Application.DTO.TradeResult
{
    public class TradeResultCreateDTO
    {
        [MaxLength(10, ErrorMessage = "The symbol must not exceed 10 characters")]
        public string Symbol { get; set; } = string.Empty;

        [Range(0.0001, 999999.9999, ErrorMessage = "Entry price must be greater than 0, and the upper limit is 999999")]
        public decimal EntryPrice { get; set; }

        [Range(0.0001, 999999.9999, ErrorMessage = "Exit price must be greater than 0, and the upper limit is 999999")]
        public decimal ExitPrice { get; set; }

        [Range(1, 100000, ErrorMessage = "Volume must be greater than 0, and the upper limit is 100000")]
        public int NumberOfStocks { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime ExitDate { get; set; }
        public TradeMethodDTO? TradeMethod { get; set; }
    }
}
