using OtpNet;
using System.Globalization;
using System.Net;

namespace LoginJWT.Services
{
    public class OtpService
    {
        public string GenerateBase32Secret()
        {
            var secret = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(secret);
        }

        public string? GenerateQrCodeValue(string? base32Secret, string? username)
        {
            if (username == null || base32Secret == null)
            {
                return null;
            }
            var uriString = new OtpUri(OtpType.Totp, base32Secret, username, "Test").ToString();
            return uriString;
        }

        public bool ValidateTotp(string? base32Secret, string totp)
        {
            var secret = Base32Encoding.ToBytes(base32Secret);
            
            // GMT (+7) only
            DateTime exactTime = GetNistTime().AddHours(-7);

            var correction = new TimeCorrection(exactTime);
            var totpValidator = new Totp(secret, timeCorrection: correction);

            bool verify = totpValidator.VerifyTotp(totp, out _);
            return verify;
        }

        public static DateTime GetNistTime()
        {
            using var response = WebRequest.Create("http://www.google.com").GetResponse();
            return DateTime.ParseExact(response.Headers["date"],
                                        "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                        CultureInfo.InvariantCulture.DateTimeFormat,
                                        DateTimeStyles.AssumeUniversal);
        }
    }
}
