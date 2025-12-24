using Fomo.Application.DTO.IndicatorsDTO;
using Fomo.Application.DTO.StockDataDTO;

namespace Fomo.Application.Services
{
    public interface IIndicatorService
    {
        ValuesAndDateDTO GetSMA(List<ValuesDTO> values, int period);

        BandsDTO GetEnvelopeBands(List<ValuesDTO> values, int period, int percentage);

        BandsDTO GetBollingerBands(List<ValuesDTO> values, int period, int k);

        StochasticDTO GetStochastic(List<ValuesDTO> values, int period, int smaperiod);

        ValuesAndDateDTO GetRSI(List<ValuesDTO> values, int period);

        ValuesAndDateDTO GetWilderRSI(List <ValuesDTO> values, int period);

        MainChannelDTO GetMainChannel(List<ValuesDTO> values);
    }
}
