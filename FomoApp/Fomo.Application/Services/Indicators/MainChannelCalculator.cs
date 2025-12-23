using Fomo.Application.DTO.IndicatorsDTO;
using Fomo.Application.DTO.StockDataDTO;

namespace Fomo.Application.Services.Indicators
{
    public class MainChannelCalculator
    {
        public MainChannelDTO CalculateMainChannel (List<ValuesDTO> values)
        {
            if (values == null || values.Count == 0)
            {
                return new MainChannelDTO
                {
                    Regression = new List<decimal>(),
                    Upper = new List<decimal>(),
                    Lower = new List<decimal>()
                };
            }

            var parser = new ParseListHelper();
            var closes = parser.ParseList(values, v => v.Close);
            var high = parser.ParseList(values, v => v.High);
            var low = parser.ParseList(values, v => v.Low);

            var n = values.Count;

            var typicalPrices = new List<decimal>();
                
            for (int i = 0; i < n; i++)
            {
                var value = (high[i] + low[i] + closes[i]) / 3;
                typicalPrices.Add(value);
            };

            var sumX = Enumerable.Range(0, n).Sum();
            var sumX2 = Enumerable.Range(0, n).Sum(x => x * x);

            var sumXY = typicalPrices
                .Select((v, i) => v * i)
                .Sum();

            var m = ((n*sumXY) - (sumX*typicalPrices.Sum()))/((n*sumX2)-(sumX*sumX));

            var b = (typicalPrices.Sum() - (m * sumX)) / n;

            var regression = new List<decimal>();

            for (int i = 0; i < n;  ++i)
            {
                var value = (m * i + b);
                regression.Add(value);
            }

            var distUp = new List<decimal>();
            var distDown = new List<decimal>();

            for (int i = 0; i < n; ++i)
            {
                var upvalue = high[i] - regression[i];
                distUp.Add(upvalue);
                var downvalue = regression[i] - low[i];
                distDown.Add(downvalue);
            }

            var maxUp = distUp.Max();
            var maxDown = distDown.Max();

            var upperList = new List<decimal>();
            var lowerList = new List<decimal>();

            for (int i = 0; i < n; ++i)
            {
                var upvalue = regression[i] + maxUp;
                upperList.Add(upvalue);
                var downvalue = regression[i] - maxDown;
                lowerList.Add(downvalue);
            }

            return new MainChannelDTO
            {
                Regression = regression,
                Upper = upperList,
                Lower = lowerList,
            };
        }
    }
}
