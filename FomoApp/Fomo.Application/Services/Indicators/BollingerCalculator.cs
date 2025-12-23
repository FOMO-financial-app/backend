using Fomo.Application.DTO.IndicatorsDTO;
using Fomo.Application.DTO.StockDataDTO;

namespace Fomo.Application.Services.Indicators
{
    public class BollingerCalculator
    {
        public BandsDTO CalculateBollinger(List<ValuesDTO> values, int period, int k)
        {
            if (values == null || values.Count < period || period == 0)
            {
                return new BandsDTO
                {
                    UpperBand = new List<decimal>(),
                    LowerBand = new List<decimal>(),
                    Date = new List<string>()
                };
            }            

            var parser = new ParseListHelper();
            var valuesd = parser.ParseList(values, v => v.Close);
            var datelist = parser.GetDate(values);
            datelist = datelist.Skip(period-1).ToList();

            var calculator = new SmaCalculator();
            var sma = calculator.CalculateSMA(valuesd, period);

            var bollingerUpper = new List<decimal>();
            var bollingerLower = new List<decimal>();
            var datetime = new List<string>();

            for (int i = period - 1; i < valuesd.Count; i++)
            {
                var subList = valuesd.GetRange(i - period + 1 , period);

                decimal variance = 0;

                var smaValue = sma[i - (period - 1)];

                foreach (var value in subList)
                {
                    variance = variance + ((value - smaValue) * (value - smaValue));
                }

                variance = variance / period;
                var stdDev = (decimal)Math.Sqrt((double)variance);

                bollingerUpper.Add(smaValue + k * stdDev);
                bollingerLower.Add(smaValue - k * stdDev);
            }

            return new BandsDTO
            {
                UpperBand = bollingerUpper,
                LowerBand = bollingerLower,
                Date = datelist
            };
        }
    }
}

