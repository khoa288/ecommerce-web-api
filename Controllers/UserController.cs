using EcommerceWebApi.Authentication;
using EcommerceWebApi.DTOs;
using EcommerceWebApi.Filters;
using EcommerceWebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebApi.Controllers
{
    [ServiceFilter(typeof(AuthorizeFilter))]
    [Controller]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserService userService,
            AuthService authService,
            ILogger<UserController> logger
        )
        {
            _userService = userService;
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("ChangeTwoFactorAuth")]
        public async Task<IActionResult> ChangeTwoFactorAuth()
        {
            try
            {
                var user = _authService.CurrentUser;

                user.IsTwoFactorAuthActivated = !user.IsTwoFactorAuthActivated;
                if (!user.IsTwoFactorAuthActivated)
                {
                    user.SecretCode = null;
                }

                var result = await _userService.UpdateUserAsync(user);
                if (result)
                {
                    _logger.LogInformation(
                        message: "User <{name}> update IsTwoFactorAuthActivated to {status}",
                        user.Username,
                        user.IsTwoFactorAuthActivated
                    );
                    return NoContent();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetUserInfo")]
        public IActionResult GetUserInfo()
        {
            try
            {
                var user = _authService.CurrentUser;

                var userDTO = new UserDTO()
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role,
                    IsTwoFactorAuthActivated = user.IsTwoFactorAuthActivated
                };

                return Ok(userDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        protected override void Dispose(bool disposing)
        {
            _userService.Dispose();
            base.Dispose(disposing);
        }
    }
}
