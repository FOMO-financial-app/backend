using Fomo.Api.Helpers;
using Fomo.Application.DTO.TradeResult;
using Fomo.Application.DTO.User;
using Fomo.Domain.Entities;
using Fomo.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fomo.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserValidateHelper _userValidateHelper;

        public UserController (IUserRepository userRepository, IUserValidateHelper userValidateHelper)
        {
            _userRepository = userRepository;
            _userValidateHelper = userValidateHelper;
        }

        [Authorize]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create()
        {
            var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;
            var name = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var email = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var picture = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value;
            
            if (auth0Id == null || name == null || email == null || picture == null)
            {
                return BadRequest("Cannot obtain user info");
            }

            var newUser = new User()
            {
                Auth0Id = auth0Id,
                Name = name,
                Email = email,
                ProfilePictureUrl = picture
            };

            await _userRepository.InsertAsync(newUser);
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
            var userData = await _userValidateHelper.GetFullUserAsync(User);
            if (userData == null) return NotFound("Invalid User");

            var tradeResultsDTO = new List<UserTradeResultDTO>();
            if (userData.TradeResults != null)
            {
                tradeResultsDTO = userData.TradeResults.Select(tr => new UserTradeResultDTO
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
                    }
                }).ToList();
            }

            var userDto = new UserDTO()
            {
                Name = userData.Name,
                Email = userData.Email,
                ProfilePictureUrl = userData.ProfilePictureUrl,
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
            var userData = await _userValidateHelper.GetOnlyUserAsync(User);
            if (userData == null) return NotFound("Invalid User");

            if (!String.IsNullOrEmpty(userUpdate.Name)) userData.Name = userUpdate.Name;
            if (userUpdate.ProfilePictureUrl != null) userData.ProfilePictureUrl = userUpdate.ProfilePictureUrl;
            if (userUpdate.SmaAlert.HasValue) userData.SmaAlert = userUpdate.SmaAlert.Value;
            if (userUpdate.BollingerAlert.HasValue) userData.BollingerAlert = userUpdate.BollingerAlert.Value;
            if (userUpdate.StochasticAlert.HasValue) userData.StochasticAlert = userUpdate.StochasticAlert.Value;
            if (userUpdate.RsiAlert.HasValue) userData.RsiAlert = userUpdate.RsiAlert.Value;

            _userRepository.UpdateAsync(userData);
            await _userRepository.SaveAsync();

            return Ok();
        }

        [Authorize]
        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete()
        {
            var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;
            if (auth0Id == null) return BadRequest("Invalid UserId");

            await _userRepository.DeleteAsync(auth0Id);
            await _userRepository.SaveAsync();

            return Ok();
        }
    }
}
