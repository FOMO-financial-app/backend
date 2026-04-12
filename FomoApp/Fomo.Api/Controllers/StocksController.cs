using Fomo.Application.DTO.StockDataDTO;
using Fomo.Application.Services;
using Fomo.Domain.Entities;
using Fomo.Infrastructure.ExternalServices.StockService;
using Fomo.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Fomo.Api.Controllers
{
    [Route("api/[controller]")]
    public class StocksController : Controller
    {
        private readonly ITwelveDataService _twelveDataService;
        private readonly IStockRepository _stockRepository;
        private readonly IIndicatorService _indicatorService;

        public StocksController(ITwelveDataService twelveDataService, IStockRepository stockRepository,
            IIndicatorService indicatorService)
        {
            _twelveDataService = twelveDataService;
            _stockRepository = stockRepository;
            _indicatorService = indicatorService;
        }

        [HttpGet("find/{query}")]
        [ProducesResponseType(typeof(SymbolAndName), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStocksFiltered(string query)
        {
            var stocks = await _stockRepository.GetFilteredStocks(query);

            if (stocks == null)
                return NotFound("Cannot obtain StockData");

            return Ok(stocks);
        }

        [HttpGet("{page:int}/{pagesize:int}")]
        [ProducesResponseType(typeof(StockPageDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStocksPage(int page, int pagesize)
        {
            if (page <= 0 || pagesize <= 0)
                return BadRequest("Page and PageSize must be greatear than 0");

            if (page <= 0 || pagesize <= 0)
                return BadRequest("Page size cannot exceed 100 items");

            var totalRecords = await _stockRepository.CountRecordsAsync();

            var totalPages = (int)Math.Ceiling((double)totalRecords / pagesize);

            if (page > totalPages)
                return Ok(new StockPageDTO
                {
                    Data = [],
                    CurrentPage = page,
                    TotalPages = totalPages
                });

            var stocks = await _stockRepository.GetPaginatedStocks(page, pagesize);

            if (stocks == null)
                return NotFound("Cannot obtain StockData");

            return Ok(new StockPageDTO
            {
                Data = stocks,
                CurrentPage = page,
                TotalPages = totalPages
            });
        }

        [HttpGet("timeseries")]
        [EnableRateLimiting("external-api")]
        [ProducesResponseType(typeof(ValuesAndChannelDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetStockTimeSeries([FromQuery] string symbol)
        {
            var timeseries = await _twelveDataService.GetTimeSeries(symbol);

            if (timeseries == null || timeseries.Values == null)
                return NotFound($"No data was found for the symbol {symbol}.");

            if (timeseries.IsRateLimited)
                return StatusCode(503, "Market data temporarily unavailable");

            var mainchannel = _indicatorService.GetMainChannel(timeseries.Values);

            var reversedValues = timeseries.Values.AsEnumerable().Reverse().ToList();

            var response = new ValuesAndChannelDTO
            {
                MetaDTO = timeseries.MetaDTO,
                Values = reversedValues,
                Regression = mainchannel.Regression,
                Upper = mainchannel.Upper,
                Lower = mainchannel.Lower,
            };

            return Ok(response);
        }
    }
}