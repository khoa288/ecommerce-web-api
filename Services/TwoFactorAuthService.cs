using OtpNet;
using System.Globalization;
using System.Net;

namespace LoginJWT.Services
{
    public class TwoFactorAuthService
    {
        private static long timeWindowUsedCurrent = new();
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

            try
            {
                var uriString = new OtpUri(OtpType.Totp, base32Secret, username, "Test").ToString();
                return uriString;
            }
            catch
            {
                throw new Exception();
            }
        }

        public bool ValidateTotp(string? base32Secret, string totp)
        {
            try
            {
                var secret = Base32Encoding.ToBytes(base32Secret);

                DateTime exactTime = GetNistTime().AddHours(-7);

                var correction = new TimeCorrection(exactTime);
                var totpValidator = new Totp(secret, timeCorrection: correction);
                bool verify = totpValidator.VerifyTotp(totp, out long timeWindowUsed, VerificationWindow.RfcSpecifiedNetworkDelay);

                if (timeWindowUsedCurrent == timeWindowUsed)
                {
                    return false;
                }
                timeWindowUsedCurrent = timeWindowUsed;

                return verify;
            }
            catch
            {
                throw new Exception();
            }
        }

        public static DateTime GetNistTime()
        {
            using var response = WebRequest.Create("http://www.google.com").GetResponse();
            try
            {
                return DateTime.ParseExact(response.Headers["date"],
                                        "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                        CultureInfo.InvariantCulture.DateTimeFormat,
                                        DateTimeStyles.AssumeUniversal);
            }
            catch (WebException ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
