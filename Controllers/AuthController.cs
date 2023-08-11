using EcommerceWebApi.Authentication;
using EcommerceWebApi.DTOs;
using EcommerceWebApi.Filters;
using Microsoft.AspNetCore.Mvc;
using static EcommerceWebApi.Authentication.AuthService;

namespace EcommerceWebApi.Controllers
{
    [ServiceFilter(typeof(AuthorizeFilter))]
    [Controller]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [AuthorizeRole("Admin")]
        [Route("ReadLog")]
        public IActionResult ReadLog([FromQuery] string logPath)
        {
            if (System.IO.File.Exists(logPath))
            {
                using var stream = System.IO.File.Open(
                    logPath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite
                );
                using var reader = new StreamReader(stream);
                return Ok(reader.ReadToEnd());
            }
            else
            {
                return NotFound();
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var result = await _authService.Login(
                    loginDTO.Username,
                    loginDTO.Password,
                    HttpContext
                );
                if (result == AuthResult.Success)
                {
                    _logger.LogInformation("User <{Name}> logged in", loginDTO.Username);
                    return NoContent();
                }
                ;
                return result switch
                {
                    AuthResult.UserNotExist => Unauthorized(),
                    AuthResult.UserExisted => BadRequest("User existed"),
                    AuthResult.InvalidCredentials => BadRequest("Invalid credentials"),
                    AuthResult.NeedSecondFactorAuth => Ok("Need second factor authentication"),
                    AuthResult.Fail => BadRequest("Login failed"),
                    _ => throw new NotImplementedException(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowFirstFactor]
        [HttpPost("LoginSecondFactor")]
        public async Task<IActionResult> LoginSecondFactor([FromBody] TotpDTO totpDTO)
        {
            try
            {
                var result = await _authService.LoginSecondFactor(totpDTO.Totp, HttpContext);
                if (result == AuthResult.Success)
                {
                    _logger.LogInformation(
                        "User <{Name}> logged in",
                        _authService.CurrentUser.Username
                    );
                    return NoContent();
                }
                ;
                return result switch
                {
                    AuthResult.UserNotExist => Unauthorized(),
                    AuthResult.UserExisted => BadRequest("User existed"),
                    AuthResult.InvalidCredentials => BadRequest("Invalid credentials"),
                    AuthResult.NeedSecondFactorAuth => Ok("Need second factor authentication"),
                    AuthResult.Fail => BadRequest("Login failed"),
                    _ => throw new NotImplementedException(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                var result = await _authService.Register(
                    registerDTO.Username,
                    registerDTO.Password
                );
                if (result == AuthResult.Success)
                {
                    _logger.LogInformation("User <{Name}> registered", registerDTO.Username);
                    return NoContent();
                }
                ;
                return result switch
                {
                    AuthResult.UserNotExist => Unauthorized(),
                    AuthResult.UserExisted => BadRequest("User existed"),
                    AuthResult.InvalidCredentials => BadRequest("Invalid credentials"),
                    AuthResult.NeedSecondFactorAuth => Ok("Need second factor authentication"),
                    AuthResult.Fail => BadRequest("Register failed"),
                    _ => throw new NotImplementedException(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("RevokeToken")]
        public async Task<IActionResult> RevokeToken()
        {
            try
            {
                var result = await _authService.RevokeToken(HttpContext);
                if (result == AuthResult.Success)
                {
                    _logger.LogInformation(
                        "User <{Name}> logged out",
                        _authService.CurrentUser.Username
                    );
                    return NoContent();
                }
                ;
                return result switch
                {
                    AuthResult.UserNotExist => Unauthorized(),
                    AuthResult.UserExisted => BadRequest("User existed"),
                    AuthResult.InvalidCredentials => BadRequest("Invalid credentials"),
                    AuthResult.NeedSecondFactorAuth => Ok("Need second factor authentication"),
                    AuthResult.Fail => BadRequest("Revoke token failed"),
                    _ => throw new NotImplementedException(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("IsAuthenticated")]
        public IActionResult IsAuthenticated()
        {
            // Return status 200 with no content
            // Return status 200 with "Need second factor authentication" if need more credentials
            // Return status 401 if user is not authenticated

            return NoContent();
        }

        [HttpGet("GetQrCode")]
        public IActionResult GetQrCode()
        {
            try
            {
                var result = _authService.GetQrCode(out string? uriString);
                return result switch
                {
                    AuthResult.Success => Ok(uriString),
                    AuthResult.UserNotExist => Unauthorized(),
                    AuthResult.UserExisted => BadRequest("User existed"),
                    AuthResult.InvalidCredentials => BadRequest("Invalid credentials"),
                    AuthResult.NeedSecondFactorAuth => Ok("Need second factor authentication"),
                    AuthResult.Fail => BadRequest("Get QR code failed"),
                    _ => throw new NotImplementedException(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowFirstFactor]
        [HttpPost("ValidateQrCode")]
        public async Task<IActionResult> ValidateQrCode([FromBody] TotpDTO totpDTO)
        {
            try
            {
                var result = await _authService.ValidateQrCode(totpDTO.QrCode, totpDTO.Totp);
                return result switch
                {
                    AuthResult.Success => NoContent(),
                    AuthResult.UserNotExist => Unauthorized(),
                    AuthResult.UserExisted => BadRequest("User existed"),
                    AuthResult.InvalidCredentials => BadRequest("Invalid credentials"),
                    AuthResult.NeedSecondFactorAuth => Ok("Need second factor authentication"),
                    AuthResult.Fail => BadRequest("Validate Qr code failed"),
                    _ => throw new NotImplementedException(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
