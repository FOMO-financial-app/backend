using Fomo.Application.DTO.IndicatorsDTO;
using Fomo.Application.DTO.StockDataDTO;

namespace Fomo.Application.Services.Indicators
{
    public class WilderRsiCalculator
    {
        public ValuesAndDateDTO CalculateWilderRsi(List<ValuesDTO> values, int period)
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

            decimal gain = 0;
            decimal loss = 0;

            for (int i = 1; i <= period; i++)
            {
                decimal diff = closeList[i] - closeList[i - 1];
                if (diff >= 0)
                {
                    gain = gain + diff;
                }
                else
                {
                    loss = loss + Math.Abs(diff);
                }
            }

            decimal avgGain = gain / period;
            decimal avgLoss = loss / period;

            decimal rs = avgLoss == 0 ? 0 : avgGain / avgLoss;
            decimal rsi = avgLoss == 0 ? 100 : 100 - (100 / (1 + rs));

            rsiList.Add(rsi);

            for (int i = period + 1; i < values.Count; i++)
            {
                decimal diff = closeList[i] - closeList[i - 1];
                if (diff >= 0)
                {
                    gain = diff;
                }
                else
                {
                    loss = Math.Abs(diff);
                }
                
                avgGain = ((avgGain * (period - 1)) + gain) / period;
                avgLoss = ((avgLoss * (period - 1)) + loss) / period;

                rs = avgLoss == 0 ? 0 : avgGain / avgLoss;
                rsi = avgLoss == 0 ? 100 : 100 - (100 / (1 + rs));

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
