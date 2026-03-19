using System.ComponentModel.DataAnnotations;

namespace Fomo.Application.DTO.TradeResult
{
    public class TradeResultCreateDTO
    {
        [MaxLength(10)]
        public string Symbol { get; set; } = string.Empty;

        [Range(0.00001, double.MaxValue, ErrorMessage = "Entry price must be greater than 0")]
        public decimal EntryPrice { get; set; }

        [Range(0.00001, double.MaxValue, ErrorMessage = "Exit price must be greater than 0")]
        public decimal ExitPrice { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Volume must be greater than 0")]
        public int NumberOfStocks { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime ExitDate { get; set; }
        public TradeMethodDTO? TradeMethod { get; set; }
    }
}
