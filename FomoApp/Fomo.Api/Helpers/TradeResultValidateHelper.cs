using Fomo.Application.DTO.TradeResult;

namespace Fomo.Api.Helpers
{
    public class TradeResultValidateHelper : ITradeResultValidateHelper
    {
        public bool IsValidTradeResultUpdateDTO(TradeResultUpdateDTO tradeResult)
        {
            if (String.IsNullOrEmpty(tradeResult.Symbol) && !tradeResult.EntryPrice.HasValue && !tradeResult.ExitPrice.HasValue &&
                !tradeResult.NumberOfStocks.HasValue && !tradeResult.EntryDate.HasValue && !tradeResult.ExitDate.HasValue) 
            {
                return false;
            }

            return true;
        }

        public bool IsValidDate(DateTime? entryDate, DateTime? exitDate, DateTime minDate)
        {
            DateTime actualDate = DateTime.Now;

            if (!entryDate.HasValue || !exitDate.HasValue) return false;
            if (entryDate < minDate || exitDate < minDate) return false;
            if (entryDate > actualDate || exitDate > actualDate) return false;
            if (entryDate > exitDate) return false;

            return true;
        }

        public bool IsEmptyTradeMethod(TradeMethodDTO tradeMethod)
        {
            if (!tradeMethod.Sma && !tradeMethod.Bollinger && !tradeMethod.Stochastic && !tradeMethod.Rsi && !tradeMethod.Other)
            {
                return false;
            }

            return true;
        }
    }
}
