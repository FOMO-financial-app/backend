using Fomo.Api.Helpers;
using Fomo.Application.DTO.TradeResult;
using Fomo.Domain.Entities;
using Fomo.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fomo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradeResultController : Controller
    {
        private readonly ITradeResultRepository _tradeResultRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IUserValidateHelper _userValidateHelper;
        private readonly ITradeResultValidateHelper _tradeResultValidateHelper;

        public TradeResultController(ITradeResultRepository tradeResultRepository, IStockRepository stockRepository,
            IUserValidateHelper userValidateHelper, ITradeResultValidateHelper tradeResultValidateHelper)
        {
            _tradeResultRepository = tradeResultRepository;
            _stockRepository = stockRepository;
            _userValidateHelper = userValidateHelper;
            _tradeResultValidateHelper = tradeResultValidateHelper;
        }

        [Authorize]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] TradeResultCreateDTO tradeResult)
        {
            if (tradeResult == null) return BadRequest("Invalid tradeResult");            

            var userId = await _userValidateHelper.GetUserIdAsync(User);
            if (userId == null) return NotFound("Invalid User");

            if (String.IsNullOrEmpty(tradeResult.Symbol)) return BadRequest("Invalid tradeResult");

            var minDate = new DateTime(2026, 1, 1, 0, 0, 0);
            if (!_tradeResultValidateHelper.IsValidDate(tradeResult.EntryDate, tradeResult.ExitDate, minDate))
            {
                return BadRequest("Invalid Date");
            }

            var symbol = await _stockRepository.GetSymbolIfExistsAsync(tradeResult.Symbol);

            var newTradeResult = new TradeResult()
            {
                Symbol = symbol,
                EntryPrice = tradeResult.EntryPrice,
                ExitPrice = tradeResult.ExitPrice,
                NumberOfStocks = tradeResult.NumberOfStocks,
                EntryDate = tradeResult.EntryDate,
                ExitDate = tradeResult.ExitDate,
                UserId = userId.Value,
                TradeMethod = tradeResult.TradeMethod == null ? new TradeMethod() : new TradeMethod
                {
                    Sma = tradeResult.TradeMethod.Sma,
                    Bollinger = tradeResult.TradeMethod.Bollinger,
                    Stochastic = tradeResult.TradeMethod.Stochastic,
                    Rsi = tradeResult.TradeMethod.Rsi,
                    Other = (!_tradeResultValidateHelper.IsEmptyTradeMethod(tradeResult.TradeMethod)) == true ||
                        tradeResult.TradeMethod.Other
                }
            };
            
            await _tradeResultRepository.InsertAsync(newTradeResult);
            await _tradeResultRepository.SaveAsync();

            return Ok("TradeResult created succesfully");
        }

        [HttpGet("{page:int}/{pagesize:int}")]
        [ProducesResponseType(typeof(TradeResultsPageDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTradeResultsPage(int page, int pagesize)
        {
            if (page <= 0 || pagesize <= 0)
                return BadRequest("Page and PageSize must be greatear than 0");

            var totalRecords = await _tradeResultRepository.CountRecordsAsync();

            var totalPages = (int)Math.Ceiling((double)totalRecords / pagesize);

            if (page > totalPages)
                return Ok(new TradeResultsPageDTO
                {
                    Data = [],
                    CurrentPage = page,
                    TotalPages = totalPages
                });

            var tradeResults = await _tradeResultRepository.GetPaginatedAsync(page, pagesize);

            if (tradeResults == null)
                return NotFound("Cannot obtain TradeResults data");

            return Ok(new TradeResultsPageDTO
            {
                Data = tradeResults,
                CurrentPage = page,
                TotalPages = totalPages
            });
        }

        [Authorize]
        [HttpGet("profile/{page:int}/{pagesize:int}")]
        [ProducesResponseType(typeof(TradeResultsPageDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTradeResultsPageByUser(int page, int pagesize)
        {
            if (page <= 0 || pagesize <= 0)
                return BadRequest("Page and PageSize must be greatear than 0");

            var userId = await _userValidateHelper.GetUserIdAsync(User);
            if (userId == null) return NotFound("Invalid User");

            var totalRecords = await _tradeResultRepository.CountRecordsByUserAsync(userId.Value);

            var totalPages = (int)Math.Ceiling((double)totalRecords / pagesize);

            if (page > totalPages)
                return Ok(new TradeResultsPageDTO
                {
                    Data = [],
                    CurrentPage = page,
                    TotalPages = totalPages
                });

            var tradeResults = await _tradeResultRepository.GetPaginatedByUserAsync(page, pagesize, userId.Value);

            if (tradeResults == null)
                return NotFound("Cannot obtain TradeResults data");

            return Ok(new TradeResultsPageDTO
            {
                Data = tradeResults,
                CurrentPage = page,
                TotalPages = totalPages
            });
        }

        [Authorize]
        [HttpPatch("edit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Edit([FromBody] TradeResultUpdateDTO Update)
        {
            if (Update == null) return BadRequest("TradeResult cannot be null");
            if (Update.TradeResultId == null) return BadRequest("Id cannot be null");
            if (!_tradeResultValidateHelper.IsValidTradeResultUpdateDTO(Update)) return BadRequest("Invalid Upadte");

            var userId = await _userValidateHelper.GetUserIdAsync(User);
            if (userId == null) return NotFound("Invalid User");

            var tradeResult = await _tradeResultRepository.GetByIdAsync(Update.TradeResultId.Value);
            if (tradeResult == null) return NotFound("TradeResult not found");

            if (tradeResult.UserId != userId.Value) return BadRequest("Only the creator can edit the post");

            if (!String.IsNullOrEmpty(Update.Symbol) && !(Update.Symbol.Length > 10))
            {
                var symbol = await _stockRepository.GetSymbolIfExistsAsync(Update.Symbol);
                tradeResult.Symbol = symbol;
            }

            if (Update.EntryPrice != null) tradeResult.EntryPrice = Update.EntryPrice.Value;
            if (Update.ExitPrice != null ) tradeResult.ExitPrice = Update.ExitPrice.Value;
            if (Update.NumberOfStocks != null) tradeResult.NumberOfStocks = Update.NumberOfStocks.Value;

            var minDate = new DateTime(2026, 1, 1, 0, 0, 0);
            if ((Update.EntryDate.HasValue && Update.ExitDate.HasValue) && 
                _tradeResultValidateHelper.IsValidDate(Update.EntryDate, Update.ExitDate, minDate))
            {
                tradeResult.EntryDate = Update.EntryDate.Value;
                tradeResult.ExitDate = Update.ExitDate.Value;
            }

            if (Update.TradeMethod != null && tradeResult.TradeMethod != null)
            {
                tradeResult.TradeMethod.Sma = Update.TradeMethod.Sma;
                tradeResult.TradeMethod.Bollinger = Update.TradeMethod.Bollinger;
                tradeResult.TradeMethod.Stochastic = Update.TradeMethod.Stochastic;
                tradeResult.TradeMethod.Rsi = Update.TradeMethod.Rsi;
                tradeResult.TradeMethod.Other = (!_tradeResultValidateHelper.IsEmptyTradeMethod(Update.TradeMethod)) == true ||
                        tradeResult.TradeMethod.Other;
            }

            await _tradeResultRepository.SaveAsync();

            return Ok();
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id < 0) return BadRequest("Invalid TradeResultId");

            var userData = await _userValidateHelper.GetUserIdAsync(User);
            if (userData == null) return NotFound("Invalid User");

            var tradeResult = await _tradeResultRepository.GetByIdAsync(id);
            if (tradeResult == null) return NotFound("TradeResult not found");

            if (tradeResult.UserId != userData) return BadRequest("Only the creator can delete this post");

            await _tradeResultRepository.DeleteIfExistsAsync(tradeResult.TradeResultId);
            await _tradeResultRepository.SaveAsync();

            return Ok("TradeResult deleted succesfully");
        }
    }
}
