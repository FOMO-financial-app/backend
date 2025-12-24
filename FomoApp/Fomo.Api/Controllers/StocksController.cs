using Fomo.Application.DTO.StockDataDTO;
using Fomo.Application.Services;
using Fomo.Domain.Entities;
using Fomo.Infrastructure.ExternalServices.MailService;
using Fomo.Infrastructure.ExternalServices.StockService;
using Fomo.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        [ProducesResponseType(typeof(SymbolAndName), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllStocks()
        {
            var stocks = await _stockRepository.GetStocks();

            if (stocks == null)
                return NotFound("Cannot obtain StockData");

            return Ok(stocks);
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

        [HttpGet("timeseries/{symbol}")]
        [ProducesResponseType(typeof(ValuesAndChannelDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStockTimeSeries(string symbol)
        {
            var timeseries = await _twelveDataService.GetTimeSeries(symbol);

            if (timeseries == null || timeseries.Values == null)
                return NotFound($"No data was found for the symbol {symbol}.");

            var mainchannel = _indicatorService.GetMainChannel(timeseries.Values);

            timeseries.Values.Reverse();

            var response = new ValuesAndChannelDTO
            {
                MetaDTO = timeseries.MetaDTO,
                Values = timeseries.Values,
                Regression = mainchannel.Regression,
                Upper = mainchannel.Upper,
                Lower = mainchannel.Lower,
            };

            return Ok(response);
        }
    }
}