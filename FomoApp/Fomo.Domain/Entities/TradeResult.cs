using System.ComponentModel.DataAnnotations;
namespace Fomo.Domain.Entities
{
    public class TradeResult
    {
        public int TradeResultId { get; set; }

        [MaxLength(10)]
        public string Symbol { get; set; } = string.Empty;
        public decimal EntryPrice { get; set; }
        public decimal ExitPrice { get; set; }
        public int NumberOfStocks { get; set; }
        public DateTime EntryDate{ get; set; }
        public DateTime ExitDate { get; set; }
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public TradeMethod TradeMethod { get; set; } = null!;
    }
}
