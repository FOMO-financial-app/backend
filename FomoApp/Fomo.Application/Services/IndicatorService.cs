using Fomo.Application.DTO.IndicatorsDTO;
using Fomo.Application.DTO.StockDataDTO;
using Fomo.Application.Services.Indicators;

namespace Fomo.Application.Services
{
    public class IndicatorService : IIndicatorService
    {

        public ValuesAndDateDTO GetSMA (List<ValuesDTO> values, int period)
        {
            var parser = new ParseListHelper();
            var valuesd = parser.ParseList(values, v => v.Close);
            var datelist = parser.GetDate(values);
            datelist = datelist.Skip(period-1).ToList();
            var calculator = new SmaCalculator();
            var sma = calculator.CalculateSMA(valuesd, period);
            return new ValuesAndDateDTO
            {
                Values = sma,
                Date = datelist
            };

        }

        public BandsDTO GetBollingerBands (List<ValuesDTO> values, int period, int k)
        {
            var calculator = new BollingerCalculator();
            return calculator.CalculateBollinger(values, period, k);
        }

        public StochasticDTO GetStochastic (List<ValuesDTO> values, int period, int smaperiod)
        {
            var calculator = new StochasticCalculator();
            return calculator.CalculateStochastic(values, period, smaperiod);
        }

        public ValuesAndDateDTO GetRSI (List<ValuesDTO> values, int period)
        {
            var calculator = new RsiCalculator();
            return calculator.CalculateRsi(values, period);
        }

        public ValuesAndDateDTO GetWilderRSI(List<ValuesDTO> values, int period)
        {
            var calculator = new WilderRsiCalculator();
            return calculator.CalculateWilderRsi(values, period);
        }

        public MainChannelDTO GetMainChannel(List<ValuesDTO> values)
        {
            var calculator = new MainChannelCalculator();
            return calculator.CalculateMainChannel(values);
        }
    }
}
