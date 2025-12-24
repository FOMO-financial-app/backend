using Fomo.Application.DTO.IndicatorsDTO;
using Fomo.Application.Services;
using Fomo.Application.Services.Indicators;
using Fomo.Infrastructure.ExternalServices.MailService;
using Fomo.Infrastructure.ExternalServices.StockService;
using Microsoft.AspNetCore.Mvc;

namespace Fomo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndicatorsController : ControllerBase
    {
        private readonly ITwelveDataService _twelveDataService;
        private readonly IIndicatorService _indicatorService;
        private readonly IAlertService _alertService;

        public IndicatorsController(ITwelveDataService twelveDataService, IIndicatorService indicatorService,
            IAlertService alertService)
        {
            _twelveDataService = twelveDataService;
            _indicatorService = indicatorService;
            _alertService = alertService;

        }

        [HttpGet("{symbol}/sma/{period:int}")]
        [ProducesResponseType(typeof(ValuesAndDateDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStockSMA(string symbol, int period)
        {
            if (period < 2)
                return BadRequest("Period must be greater than one.");

            var timeseries = await _twelveDataService.GetTimeSeries(symbol);

            if (timeseries == null || timeseries.Values == null)
                return NotFound($"No data was found for the symbol {symbol}.");

            if (period > timeseries.Values.Count)
                return BadRequest("Period cannot exceed the number of elements.");

            var sma = _indicatorService.GetSMA(timeseries.Values, period);

            var parser = new ParseListHelper();
            var closes = parser.ParseList(timeseries.Values, v => v.Close);

            string indicator = $"SMA with a period of {period}";
            await _alertService.SendSmaAlert(closes, sma.Values, symbol, indicator);

            return Ok(sma);
        }

        [HttpGet("{symbol}/envelope/{period:int}/{percentage:int}")]
        [ProducesResponseType(typeof(BandsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStockEnvelopeBands(string symbol, int period, int percentage)
        {
            if (period < 5)
                return BadRequest("Period must be at least five.");

            if (percentage < 1 || percentage > 15)
                return BadRequest("K must be greater than zero and less than or equal to fifteen.");

            var timeseries = await _twelveDataService.GetTimeSeries(symbol);

            if (timeseries == null || timeseries.Values == null)
                return NotFound($"No data was found for the symbol {symbol}.");

            if (period > timeseries.Values.Count)
                return BadRequest("Period cannot exceed the number of elements.");

            var envelopeBands = _indicatorService.GetEnvelopeBands(timeseries.Values, period, percentage);

            return Ok(envelopeBands);
        }

        [HttpGet("{symbol}/bollinger/{period:int}/{k:int}")]
        [ProducesResponseType(typeof(BandsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStockBollingerBands(string symbol, int period, int k)
        {
            if (period < 5)
                return BadRequest("Period must be at least five.");

            if (k < 1 || k > 5)
                return BadRequest("K must be greater than zero and less than or equal to five.");

            var timeseries = await _twelveDataService.GetTimeSeries(symbol);

            if (timeseries == null || timeseries.Values == null)
                return NotFound($"No data was found for the symbol {symbol}.");

            if (period > timeseries.Values.Count)
                return BadRequest("Period cannot exceed the number of elements.");

            var bollingerBands = _indicatorService.GetBollingerBands(timeseries.Values, period, k);

            var parser = new ParseListHelper();
            var closes = parser.ParseList(timeseries.Values, v => v.Close);

            string indicator = $"Bollinger Bands with a period of {period} and a k of {k}";
            await _alertService.SendBollingerAlert(closes, bollingerBands.LowerBand, symbol, indicator);

            return Ok(bollingerBands);
        }

        [HttpGet("{symbol}/stochastic/{period:int}/{smaperiod:int}")]
        [ProducesResponseType(typeof(StochasticDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStochasticOscillator(string symbol, int period, int smaperiod)
        {
            if (period < 5)
                return BadRequest("Period must be at least five.");

            if (smaperiod < 1 || smaperiod > period)
                return BadRequest("smaperiod must be greater than zero and less than period.");

            var timeseries = await _twelveDataService.GetTimeSeries(symbol);

            if (timeseries == null || timeseries.Values == null)
                return NotFound($"No data was found for the symbol {symbol}.");

            if (period > timeseries.Values.Count)
                return BadRequest("Period cannot exceed the number of elements.");

            var stochasticOscillator = _indicatorService.GetStochastic(timeseries.Values, period, smaperiod);

            string indicator = $"Stochastic Oscilator with a period of {period} and a SMA period of {smaperiod}";
            await _alertService.SendStochasticAlert(stochasticOscillator.K, stochasticOscillator.D, symbol, indicator);

            return Ok(stochasticOscillator);
        }

        [HttpGet("{symbol}/rsi/{period:int}")]
        [ProducesResponseType(typeof(ValuesAndDateDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStockRSI(string symbol, int period)
        {
            if (period < 7 || period > 14)
                return BadRequest("Period must be between 7 and 14.");

            var timeseries = await _twelveDataService.GetTimeSeries(symbol);

            if (timeseries == null || timeseries.Values == null)
                return NotFound($"No data was found for the symbol {symbol}.");

            if (period > timeseries.Values.Count)
                return BadRequest("Period cannot exceed the number of elements.");

            var rsi = _indicatorService.GetRSI(timeseries.Values, period);

            string indicator = $"RSI with a period of {period}";
            await _alertService.SendRsiAlert(rsi.Values, symbol, indicator);

            return Ok(rsi);
        }

        [HttpGet("{symbol}/wrsi/{period:int}")]
        [ProducesResponseType(typeof(ValuesAndDateDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStockWilderRSI(string symbol, int period)
        {
            if (period < 7 || period > 14)
                return BadRequest("Period must be between 9 and 25.");

            var timeseries = await _twelveDataService.GetTimeSeries(symbol);

            if (timeseries == null || timeseries.Values == null)
                return NotFound($"No data was found for the symbol {symbol}.");

            if (period > timeseries.Values.Count)
                return BadRequest("Period cannot exceed the number of elements.");

            var wrsi = _indicatorService.GetWilderRSI(timeseries.Values, period);

            string indicator = $"Wilder RSI with a period of {period}";
            await _alertService.SendRsiAlert(wrsi.Values, symbol, indicator);

            return Ok(wrsi);
        }
    }
}

