using LoginJWT.Models;
using LoginJWT.Services;
using LoginJWT.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace LoginJWT.Controllers
{
    [Controller]
    public class AuthController : Controller
    {
        private readonly AppSettings _applicationSettings;
        private readonly UserService _userService;
        private readonly TwoFactorAuthService _twoFactorAuthService;
        private readonly JWTHelper _jwt;

        public AuthController(IOptions<AppSettings> applicationSettings, UserService userService, TwoFactorAuthService twoFactorAuthService)
        {
            _applicationSettings = applicationSettings.Value;
            _userService = userService;
            _twoFactorAuthService = twoFactorAuthService;
            _jwt = new JWTHelper(_applicationSettings, _userService);
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] Login model)
        {
            var user = _userService.GetUser(username: model.UserName);

            if (user == null)
            {
                return BadRequest("Username Or Password Was Invalid");
            }

            // Check password & two-factor authentication
            using HMACSHA512? hmac = new(key: user.PasswordSalt);
            var compute = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.Password));

            var match = compute.SequenceEqual(user.PasswordHash);
            if (!match)
            {
                return BadRequest("Username Or Password Was Invalid");
            }
            else if (user.IsTwoFactorAuthActivated)
            {
                user.IsFirstFactorChecked = true;
                return Ok("Unauthorized");
            }

            _jwt.JWTGenerator(user, HttpContext);

            return Ok("Success");
        }

        [HttpPost("LoginSecondFactor")]
        public IActionResult LoginSecondFactor([FromBody] TwoFactorAuth model)
        {
            var user = _userService.GetUser(username: model.UserName);
            if (user == null || !user.IsTwoFactorAuthActivated || !user.IsFirstFactorChecked)
            {
                return BadRequest("Unauthorized");
            }
            else
            {
                bool validated = _twoFactorAuthService.ValidateTotp(base32Secret: user.SecretCode, totp: model.Totp);
                if (!validated)
                {
                    return BadRequest("Invalid OTP");
                }
                _jwt.JWTGenerator(user, HttpContext);
                user.IsFirstFactorChecked = false;
                return Ok("Success");
            }
        }

        [HttpGet("RefreshToken")]
        public ActionResult<string> RefreshToken()
        {
            var result = _jwt.RefreshToken(HttpContext);
            switch (result.ResultCode)
            {
                case RefreshTokenResult.Success:
                    return Ok("Success");

                case RefreshTokenResult.UserNotExist:
                    return BadRequest("UserNotExist");

                case RefreshTokenResult.TokenExpire:
                    _jwt.RevokeToken(result.User.UserName, HttpContext);
                    return BadRequest("TokenExpire");

                case RefreshTokenResult.Other:
                default:
                    return BadRequest("Fail");
            }
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [HttpDelete("RevokeToken/{username}")]
        public IActionResult RevokeToken(string username)
        {
            try
            {
                _jwt.RevokeToken(username, HttpContext);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] Register model)
        {
            var user = new User { UserName = model.Username };

            if (model.ConfirmPassword == model.Password)
            {
                using (HMACSHA512? hmac = new())
                {
                    user.PasswordSalt = hmac.Key;
                    user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.Password));
                }

                user.IsFirstFactorChecked = false;
                user.IsTwoFactorAuthActivated = false;
            }
            else
            {
                return BadRequest("Passwords Don't Match");
            }

            _userService.AddUser(user);

            return Ok();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [HttpGet("GetQrCodeValue")]
        public IActionResult GetSecretCode()
        {
            var username = _jwt.GetUsername(HttpContext);
            var user = _userService.GetUser(username);
            if (user == null)
            {
                return BadRequest("Unauthorized");
            }
            user.SecretCode = _twoFactorAuthService.GenerateBase32Secret();
            var uriString = _twoFactorAuthService.GenerateQrCodeValue(base32Secret: user.SecretCode, username: username);
            return Ok(uriString);
        }

        [HttpPost("ValidateTotp")]
        public IActionResult ValidateTotp([FromBody] TwoFactorAuth model)
        {
            var username = _jwt.GetUsername(HttpContext);

            var user = _userService.GetUser(username);
            if (user == null)
            {
                return BadRequest("Unauthorized");
            }

            bool validated = _twoFactorAuthService.ValidateTotp(base32Secret: user.SecretCode, totp: model.Totp);
            if (!validated)
            {
                return BadRequest("Invalid OTP");
            }

            return Ok();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [HttpPost("ChangeTwoFactorAuth")]
        public IActionResult ChangeTwoFactorAuth()
        {
            var username = _jwt.GetUsername(HttpContext);

            var user = _userService.GetUser(username);
            if (user == null)
            {
                return BadRequest("Unauthorized");
            }

            _userService.UpdateTwoFactorAuth(user);
            return Ok();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [HttpGet("GetTwoFactorAuth")]
        public IActionResult GetTwoFactorAuthStatus()
        {
            var username = _jwt.GetUsername(HttpContext);

            var user = _userService.GetUser(username);
            if (user == null)
            {
                return BadRequest("Unauthorized");
            }

            var status = user.IsTwoFactorAuthActivated;
            return Ok(status.ToString());
        }
    }
}
