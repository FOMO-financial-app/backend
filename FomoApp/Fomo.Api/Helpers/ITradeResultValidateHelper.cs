using Fomo.Application.DTO.TradeResult;
using Fomo.Domain.Entities;

namespace Fomo.Api.Helpers
{
    public interface ITradeResultValidateHelper
    {
        bool IsValidTradeResultUpdateDTO(TradeResultUpdateDTO tradeResult);
        bool IsValidDate(DateTime? entryDate, DateTime? exitDate, DateTime minDate);
        bool IsEmptyTradeMethod(TradeMethodDTO tradeMethod);
    }
}
