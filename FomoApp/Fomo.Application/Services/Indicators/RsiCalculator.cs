using Fomo.Application.DTO.IndicatorsDTO;
using Fomo.Application.DTO.StockDataDTO;

namespace Fomo.Application.Services.Indicators
{
    public class RsiCalculator
    {
        public ValuesAndDateDTO CalculateRsi(List<ValuesDTO> values, int period)
        {
            if (values == null || values.Count < period + 1 || period == 0)
            {
                return new ValuesAndDateDTO
                {
                    Values = new List<decimal>(),
                    Date = new List<string>()
                };
            }

            var parser = new ParseListHelper();
            var closeList = parser.ParseList(values, v => v.Close);
            var datelist = parser.GetDate(values);
            datelist = datelist.Skip(period).ToList();

            var rsiList = new List<decimal>();

            for (int i = period; i < values.Count; i++)
            {
                decimal gain = 0;
                decimal loss = 0;

                for (int j = i - period + 1; j <= i ; j++)
                {
                    decimal diff = closeList[j] - closeList[j-1];
                    if (diff >= 0)
                    {
                        gain = gain + diff;
                    }
                    else
                    {
                        loss = loss + Math.Abs(diff);
                    }
                }

                gain = gain/period;
                loss = loss/period;

                decimal rs = loss == 0 ? 0 : gain / loss;
                decimal rsi = loss == 0 ? 100 : 100 - (100 / (1 + rs));

                rsiList.Add(rsi);
            }
            return new ValuesAndDateDTO
            {
                Values = rsiList,
                Date = datelist
            };
        }
    }
}
