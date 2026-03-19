using Fomo.Application.DTO.TradeResult;

namespace Fomo.Api.Helpers
{
    public interface ITradeResultValidateHelper
    {
        bool IsValidTradeResultUpdateDTO(TradeResultUpdateDTO tradeResult);
        bool IsValidDate(DateTime? entryDate, DateTime? exitDate, DateTime minDate);
    }
}
