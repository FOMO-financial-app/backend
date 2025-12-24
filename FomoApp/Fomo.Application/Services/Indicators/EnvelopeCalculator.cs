using Fomo.Application.DTO.IndicatorsDTO;
using Fomo.Application.DTO.StockDataDTO;

namespace Fomo.Application.Services.Indicators
{
    public class EnvelopeCalculator
    {
        public BandsDTO CalculateEnvelope(List<ValuesDTO> values, int period, int percentage)
        {
            if (values == null || values.Count < period || period == 0 || percentage < 1)
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
            datelist = datelist.Skip(period - 1).ToList();

            var calculator = new SmaCalculator();
            var sma = calculator.CalculateSMA(valuesd, period);

            var envolopeUpper = new List<decimal>();
            var envolopeLower = new List<decimal>();

            for (int i = 0; i < sma.Count; i++)
            {
                var upperValue = sma[i] * (1+percentage/ 100.0m);
                var lowerValue = sma[i] * (1-percentage/ 100.0m);
                envolopeUpper.Add(upperValue);
                envolopeLower.Add(lowerValue);
            }

            return new BandsDTO
            {
                UpperBand = envolopeUpper,
                LowerBand = envolopeLower,
                Date = datelist
            };
        }
    }
}
