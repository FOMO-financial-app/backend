using Fomo.Application.DTO.IndicatorsDTO;
using Fomo.Application.Services;
using Fomo.Application.Services.Indicators;
using Fomo.Infrastructure.ExternalServices.MailService;
using Fomo.Infrastructure.ExternalServices.StockService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Fomo.Api.Controllers
{
    [Route("api/[controller]")]
    [EnableRateLimiting("external-api")]
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

        [HttpGet("sma")]
        [ProducesResponseType(typeof(ValuesAndDateDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetStockSMA([FromQuery] string symbol, [FromQuery] int period)
        {
            if (period < 2)
                return BadRequest("Period must be greater than one.");

            var timeseries = await _twelveDataService.GetTimeSeries(symbol);

            if (timeseries == null || timeseries.Values == null)
                return NotFound($"No data was found for the symbol {symbol}.");

            if (timeseries.IsRateLimited)
                return StatusCode(503, "Market data temporarily unavailable");

            if (period > timeseries.Values.Count)
                return BadRequest("Period cannot exceed the number of elements.");

            var sma = _indicatorService.GetSMA(timeseries.Values, period);

            var parser = new ParseListHelper();
            var closes = parser.ParseList(timeseries.Values, v => v.Close);

            string indicator = $"SMA con un período de {period}";
            await _alertService.SendSmaAlert(closes, sma.Values, symbol, indicator);

            return Ok(sma);
        }

        [HttpGet("envelopes")]
        [ProducesResponseType(typeof(BandsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetStockEnvelopeBands([FromQuery] string symbol, [FromQuery] int period, 
            [FromQuery] int percentage)
        {
            if (period < 5)
                return BadRequest("Period must be at least five.");

            if (percentage < 1 || percentage > 15)
                return BadRequest("K must be greater than zero and less than or equal to fifteen.");

            var timeseries = await _twelveDataService.GetTimeSeries(symbol);

            if (timeseries == null || timeseries.Values == null)
                return NotFound($"No data was found for the symbol {symbol}.");

            if (timeseries.IsRateLimited)
                return StatusCode(503, "Market data temporarily unavailable");

            if (period > timeseries.Values.Count)
                return BadRequest("Period cannot exceed the number of elements.");

            var envelopeBands = _indicatorService.GetEnvelopeBands(timeseries.Values, period, percentage);

            return Ok(envelopeBands);
        }

        [HttpGet("bollinger")]
        [ProducesResponseType(typeof(BandsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetStockBollingerBands([FromQuery] string symbol, [FromQuery] int period,
            [FromQuery] int k)
        {
            if (period < 5)
                return BadRequest("Period must be at least five.");

            if (k < 1 || k > 5)
                return BadRequest("K must be greater than zero and less than or equal to five.");

            var timeseries = await _twelveDataService.GetTimeSeries(symbol);

            if (timeseries == null || timeseries.Values == null)
                return NotFound($"No data was found for the symbol {symbol}.");

            if (timeseries.IsRateLimited)
                return StatusCode(503, "Market data temporarily unavailable");

            if (period > timeseries.Values.Count)
                return BadRequest("Period cannot exceed the number of elements.");

            var bollingerBands = _indicatorService.GetBollingerBands(timeseries.Values, period, k);

            var parser = new ParseListHelper();
            var closes = parser.ParseList(timeseries.Values, v => v.Close);

            string indicator = $"Bandas de Bollinger con un período de {period} y k={k}";
            await _alertService.SendBollingerAlert(closes, bollingerBands.LowerBand, symbol, indicator);

            return Ok(bollingerBands);
        }

        [HttpGet("stochastic")]
        [ProducesResponseType(typeof(StochasticDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetStochasticOscillator([FromQuery] string symbol, [FromQuery] int period,
            [FromQuery] int smaperiod)
        {
            if (period < 5)
                return BadRequest("Period must be at least five.");

            if (smaperiod < 1 || smaperiod > period)
                return BadRequest("smaperiod must be greater than zero and less than period.");

            var timeseries = await _twelveDataService.GetTimeSeries(symbol);

            if (timeseries == null || timeseries.Values == null)
                return NotFound($"No data was found for the symbol {symbol}.");

            if (timeseries.IsRateLimited)
                return StatusCode(503, "Market data temporarily unavailable");

            if (period > timeseries.Values.Count)
                return BadRequest("Period cannot exceed the number of elements.");

            var stochasticOscillator = _indicatorService.GetStochastic(timeseries.Values, period, smaperiod);

            string indicator = $"Oscilador Estocástico con un período de {period} utilizando un SMA con período de {smaperiod}";
            await _alertService.SendStochasticAlert(stochasticOscillator.K, stochasticOscillator.D, symbol, indicator);

            return Ok(stochasticOscillator);
        }

        [HttpGet("rsi")]
        [ProducesResponseType(typeof(ValuesAndDateDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetStockRSI([FromQuery] string symbol, [FromQuery] int period)
        {
            if (period < 7 || period > 14)
                return BadRequest("Period must be between 7 and 14.");

            var timeseries = await _twelveDataService.GetTimeSeries(symbol);

            if (timeseries == null || timeseries.Values == null)
                return NotFound($"No data was found for the symbol {symbol}.");

            if (timeseries.IsRateLimited)
                return StatusCode(503, "Market data temporarily unavailable");

            if (period > timeseries.Values.Count)
                return BadRequest("Period cannot exceed the number of elements.");

            var rsi = _indicatorService.GetRSI(timeseries.Values, period);

            string indicator = $"RSI con un período de {period}";
            await _alertService.SendRsiAlert(rsi.Values, symbol, indicator);

            return Ok(rsi);
        }

        [HttpGet("wrsi")]
        [ProducesResponseType(typeof(ValuesAndDateDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetStockWilderRSI([FromQuery] string symbol, [FromQuery] int period)
        {
            if (period < 7 || period > 14)
                return BadRequest("Period must be between 9 and 25.");

            var timeseries = await _twelveDataService.GetTimeSeries(symbol);

            if (timeseries == null || timeseries.Values == null)
                return NotFound($"No data was found for the symbol {symbol}.");

            if (timeseries.IsRateLimited)
                return StatusCode(503, "Market data temporarily unavailable");

            if (period > timeseries.Values.Count)
                return BadRequest("Period cannot exceed the number of elements.");

            var wrsi = _indicatorService.GetWilderRSI(timeseries.Values, period);

            string indicator = $"RSI de Wilder con un período de of {period}";
            await _alertService.SendRsiAlert(wrsi.Values, symbol, indicator);

            return Ok(wrsi);
        }
    }
}

