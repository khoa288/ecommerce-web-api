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
        private readonly UserService _userService = new UserService();
        private readonly OtpService _otpService = new OtpService();
        private readonly JWTHelper _jwt;

        public AuthController(IOptions<AppSettings> applicationSettings, HttpClient httpClient)
        {
            this._applicationSettings = applicationSettings.Value;
            _jwt = new JWTHelper(this._applicationSettings, httpClient);
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
            if (user == null || !user.IsTwoFactorAuthActivated)
            {
                return BadRequest("Unauthorized");
            } 
            else
            {
                bool validated = _otpService.ValidateTotp(base32Secret: user.SecretCode, totp: model.Totp);
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
                    return Ok("UserNotExist");

                case RefreshTokenResult.TokenExpire:
                    _jwt.RevokeToken(result.User.UserName, HttpContext);
                    return Ok("TokenExpire");

                case RefreshTokenResult.Other:
                default:
                    return Ok("Fail");
            }
        }

        [HttpDelete("RevokeToken/{username}")]
        public IActionResult RevokeToken(string username)
        {
            _jwt.RevokeToken(username, HttpContext);
            return Ok();
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] Register model)
        {
            var user = new User { UserName = model.UserName };

            if (model.ConfirmPassword == model.Password)
            {
                using (HMACSHA512? hmac = new HMACSHA512())
                {
                    user.PasswordSalt = hmac.Key;
                    user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.Password));
                }
                user.SecretCode = _otpService.GenerateBase32Secret();
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

        [HttpGet("GetUserName")]
        public IActionResult GetUserName()
        {
            var username = _jwt.GetUsername(HttpContext);
            return Ok(username);
        }

        [HttpGet("GetQrCodeValue")]
        public IActionResult GetSecretCode()
        {
            var username = _jwt.GetUsername(HttpContext);
            var user = _userService.GetUser(username);
            if (user == null)
            {
                return BadRequest("Unauthorized");
            }
            var uriString = _otpService.GenerateQrCodeValue(base32Secret: user.SecretCode, username: username);
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

            bool validated = _otpService.ValidateTotp(base32Secret: user.SecretCode, totp: model.Totp);
            if (!validated)
            {
                return BadRequest("Invalid OTP");
            }

            return Ok();
        }

        [HttpPost("ChangeTwoFactorAuth")]
        public IActionResult ChangeTwoFactorAuth()
        {
            var username = _jwt.GetUsername(HttpContext);
            var user = _userService.GetUser(username);
            if (user == null)
            {
                return BadRequest("Unauthorized");
            }
            user.SecretCode = _otpService.GenerateBase32Secret();
            _userService.UpdateTwoFactorAuth(user);

            return Ok();
        }

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
