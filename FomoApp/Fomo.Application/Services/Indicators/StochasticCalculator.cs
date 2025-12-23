using Fomo.Application.DTO.IndicatorsDTO;
using Fomo.Application.DTO.StockDataDTO;

namespace Fomo.Application.Services.Indicators
{
    public class StochasticCalculator
    {
        public StochasticDTO CalculateStochastic(List<ValuesDTO> values, int period, int smaperiod) 
        {
            if (values == null || values.Count < period || values.Count - period +1 < smaperiod)
            {
                return new StochasticDTO
                {
                    K = new List<decimal>(),
                    D = new List<decimal>()
                };
            }

            var parser = new ParseListHelper();
            var lowList = parser.ParseList(values, v => v.Low);
            var highList = parser.ParseList(values, v => v.High);
            var close = parser.ParseList(values, v => v.Close);
            var kdatelist = parser.GetDate(values);
            kdatelist = kdatelist.Skip(period - 1).ToList();
            var ddatelist = parser.GetDate(values);
            ddatelist = ddatelist.Skip(period+smaperiod-2).ToList();

            var calculator = new SmaCalculator();            

            var stochasticK = new List<decimal>();
            var stochasticD = new List<decimal>();

            for (int i = period - 1; i < values.Count; i++)
            {
                var lows = lowList.GetRange(i - period + 1 , period);
                var highs = highList.GetRange(i - period + 1, period);

                decimal min = lows.Min();
                decimal max = highs.Max();

                decimal c = close[i];

                decimal divisor = max - min;

                decimal kPoint = divisor == 0 ? 0 : ((c - min)/divisor)*100;
                stochasticK.Add(kPoint);
            }

            stochasticD = calculator.CalculateSMA(stochasticK, smaperiod);

            return new StochasticDTO 
            {
                K = stochasticK,
                Kdate = kdatelist,
                D = stochasticD,
                Ddate = ddatelist
            };
        }
    }
}
