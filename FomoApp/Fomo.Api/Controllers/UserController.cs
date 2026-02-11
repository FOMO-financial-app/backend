using Fomo.Application.DTO.TradeResult;
using Fomo.Application.DTO.User;
using Fomo.Domain.Entities;
using Fomo.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fomo.Api.Controllers
{    
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController (IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Authorize]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] UserCreateDTO userCreate)
        {
            var auth0Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            var name = userCreate.Name;
            var email = userCreate.Email;

            if (String.IsNullOrEmpty(auth0Id) || String.IsNullOrEmpty(name) || String.IsNullOrEmpty(email))
            {
                return BadRequest("Cannot obtain user info");
            }

            var newUser = new User()
            {
                Auth0Id = auth0Id,
                Name = name,
                Email = email
            };

            await _userRepository.InsertIfNotExistsAsync(newUser);
            await _userRepository.SaveAsync();

            return Ok();
        }

        [Authorize]
        [HttpGet("details")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Details()
        {
            var auth0Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;

            if (auth0Id == null) return BadRequest("User must be authenticated");

            var userData = await _userRepository.GetByAuth0IdAsync(auth0Id);

            if (userData == null) return NotFound("Invalid User");

            var tradeResultsDTO = new List<TradeResultDTO>();
            if (userData.TradeResults != null)
            {
                tradeResultsDTO = userData.TradeResults.Select(tr => new TradeResultDTO
                {
                    TradeResultId = tr.TradeResultId,
                    Symbol = tr.Symbol,
                    EntryPrice = tr.EntryPrice,
                    ExitPrice = tr.ExitPrice,
                    Profit = tr.Profit,
                    NumberOfStocks = tr.NumberOfStocks,
                    EntryDate = tr.EntryDate,
                    ExitDate = tr.ExitDate,
                    TradeMethod = new TradeMethodDTO
                    {
                        Sma = tr.TradeMethod?.Sma ?? false,
                        Bollinger = tr.TradeMethod?.Bollinger ?? false,
                        Stochastic = tr.TradeMethod?.Stochastic ?? false,
                        Rsi = tr.TradeMethod?.Rsi ?? false,
                        Other = tr.TradeMethod?.Other ?? false,
                    },
                    UserName = userData.Name,
                }).ToList();
            }

            var userDto = new UserDTO()
            {
                Name = userData.Name,
                SmaAlert = userData.SmaAlert,
                BollingerAlert = userData.BollingerAlert,
                StochasticAlert = userData.StochasticAlert,
                RsiAlert = userData.RsiAlert,
                TradeResults = tradeResultsDTO,
            };

            return Ok(userDto);
        }

        [Authorize]
        [HttpPatch("edit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Edit([FromBody] UserUpdateDTO userUpdate)
        {
            var auth0Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;

            if (auth0Id == null) return BadRequest("User must be authenticated");

            var userData = await _userRepository.GetOnlyUserByAuth0IdAsync(auth0Id);

            if (userData == null) return NotFound("Invalid User");

            if (!String.IsNullOrEmpty(userUpdate.Name)) userData.Name = userUpdate.Name;
            if (userUpdate.SmaAlert.HasValue) userData.SmaAlert = userUpdate.SmaAlert.Value;
            if (userUpdate.BollingerAlert.HasValue) userData.BollingerAlert = userUpdate.BollingerAlert.Value;
            if (userUpdate.StochasticAlert.HasValue) userData.StochasticAlert = userUpdate.StochasticAlert.Value;
            if (userUpdate.RsiAlert.HasValue) userData.RsiAlert = userUpdate.RsiAlert.Value;

            await _userRepository.SaveAsync();

            return Ok();
        }

        [Authorize]
        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete()
        {
            var auth0Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;

            if (auth0Id == null) return BadRequest("Invalid UserId");

            await _userRepository.DeleteIfExistsAsync(auth0Id);
            await _userRepository.SaveAsync();

            return Ok();
        }
    }
}
