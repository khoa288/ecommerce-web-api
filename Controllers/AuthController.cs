using LoginJWT.Entities;
using LoginJWT.Models;
using LoginJWT.Services;
using LoginJWT.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace LoginJWT.Controllers
{
    [ServiceFilter(typeof(AuthorizeFilter))]
    [Controller]
    public class AuthController : Controller
    {
        private readonly AppSettings _applicationSettings;
        private readonly UserService _userService;
        private readonly SecondFactorAuthService _twoFactorAuthService;
        private readonly JWTHelper _jwt;

        public AuthController(
            IOptions<AppSettings> applicationSettings,
            UserService userService,
            SecondFactorAuthService twoFactorAuthService
        )
        {
            _applicationSettings = applicationSettings.Value;
            _userService = userService;
            _twoFactorAuthService = twoFactorAuthService;
            _jwt = new JWTHelper(_applicationSettings, _userService);
        }

        private User? GetUserFromJWT()
        {
            var username = _jwt.GetUsername(HttpContext);
            var user = _userService.GetUser(username: username);
            return user;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest model)
        {
            try
            {
                // Check username
                var user = _userService.GetUser(username: model.UserName);
                if (user == null)
                {
                    return BadRequest("Username or Password is invalid");
                }

                // Check password & two-factor authentication
                using HMACSHA512? hmac = new(key: user.PasswordSalt);
                var compute = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.Password));
                var match = compute.SequenceEqual(user.PasswordHash);
                if (!match)
                {
                    return BadRequest("Username or Password is invalid");
                }
                else if (user.IsTwoFactorAuthActivated)
                {
                    // Generate JWT requiring second auth factor
                    _jwt.JWTGenerator(user, isSecondFactorChecked: false, HttpContext);
                    return Ok("Unauthorized");
                }

                // Generate JWT providing access
                _jwt.JWTGenerator(user, isSecondFactorChecked: true, HttpContext);

                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowFirstFactor]
        [HttpPost("LoginSecondFactor")]
        public IActionResult LoginSecondFactor([FromBody] SecondFactorAuthRequest model)
        {
            try
            {
                // If request doesn't contain username
                if (model.UserName == null)
                {
                    model.UserName = _jwt.GetUsername(HttpContext);
                }

                // Check username
                var user = _userService.GetUser(username: model.UserName);
                if (user == null)
                {
                    return BadRequest("Unauthorized");
                }

                // Validate TOTP
                bool validated = _twoFactorAuthService.ValidateTotp(
                    base32Secret: user.SecretCode,
                    totp: model.Totp
                );
                if (!validated)
                {
                    return BadRequest("Invalid OTP");
                }

                // Revoke and generate new JWT providing access
                _jwt.RevokeToken(user, HttpContext);
                _jwt.JWTGenerator(user, isSecondFactorChecked: true, HttpContext);

                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterRequest model)
        {
            try
            {
                // Check if username is taken
                var username = model.UserName;
                if (_userService.GetUser(username: username) != null)
                {
                    return BadRequest("Username is not available");
                }

                // Validate password & construct new user
                var user = new User { UserName = username };
                if (model.ConfirmPassword == model.Password)
                {
                    using (HMACSHA512? hmac = new())
                    {
                        user.PasswordSalt = hmac.Key;
                        user.PasswordHash = hmac.ComputeHash(
                            System.Text.Encoding.UTF8.GetBytes(model.Password)
                        );
                    }

                    user.IsTwoFactorAuthActivated = false;
                }
                else
                {
                    return BadRequest("Passwords don't match");
                }

                // Add new user
                _userService.AddUser(user);

                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("RevokeToken")]
        public IActionResult RevokeToken()
        {
            try
            {
                // Get user from JWT
                var user = GetUserFromJWT();
                if (user == null)
                {
                    return BadRequest("Unauthorized");
                }

                // Revoke all tokens
                _jwt.RevokeToken(user, HttpContext);
                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("IsAuthenticated")]
        public IActionResult IsAuthenticated()
        {
            // Return status 200 with no content if user is authenticated
            // Return status 200 with "Unauthorized" if user needs second factor auth
            // Return status 401 if user is not authenticated
            return Ok();
        }

        [HttpGet("GetQrCode")]
        public IActionResult GetQrCode()
        {
            try
            {
                // Get user from JWT
                var user = GetUserFromJWT();
                if (user == null)
                {
                    return BadRequest("Unauthorized");
                }

                // Generate secret & QR code
                var uriString = _twoFactorAuthService.GenerateQrCode(
                    base32Secret: _twoFactorAuthService.GenerateBase32Secret(),
                    username: user.UserName
                );
                return Ok(uriString);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("ValidateQrCode")]
        public IActionResult ValidateQrCode([FromBody] SecondFactorAuthRequest model)
        {
            try
            {
                // If request doesn't contain username
                if (model.UserName == null)
                {
                    model.UserName = _jwt.GetUsername(HttpContext);
                }

                // Check username
                var user = _userService.GetUser(username: model.UserName);
                if (user == null)
                {
                    return BadRequest("Unauthorized");
                }

                // Check if request contains qrcode value
                if (model.QrCode == null)
                {
                    return BadRequest("Unauthorized");
                }

                // Validate and extract secret
                string? secret = _twoFactorAuthService.GetSecretFromQrCode(qrCode: model.QrCode);
                if (secret == null)
                {
                    return BadRequest("Invalid QR code");
                }

                // Validate TOTP
                bool validated = _twoFactorAuthService.ValidateTotp(
                    base32Secret: secret,
                    totp: model.Totp
                );
                if (!validated)
                {
                    return BadRequest("Invalid OTP");
                }

                // Update user's secret code
                user.SecretCode = secret;

                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("ChangeTwoFactorAuth")]
        public IActionResult ChangeTwoFactorAuth()
        {
            try
            {
                // Get user from JWT
                var user = GetUserFromJWT();
                if (user == null)
                {
                    return BadRequest("Unauthorized");
                }

                // Change user's two factor auth option
                _userService.UpdateTwoFactorAuth(user);
                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetTwoFactorAuth")]
        public IActionResult GetTwoFactorAuthStatus()
        {
            try
            {
                // Get user form JWT
                var user = GetUserFromJWT();
                if (user == null)
                {
                    return BadRequest("Unauthorized");
                }

                // Get user's two factor auth option
                var status = user.IsTwoFactorAuthActivated;
                return Ok(status.ToString());
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
