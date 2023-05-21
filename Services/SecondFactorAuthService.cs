using OtpNet;
using System.Globalization;

namespace LoginJWT.Services
{
    public class SecondFactorAuthService
    {
        private static long timeWindowUsedCurrent = new();

        public string GenerateBase32Secret()
        {
            var secret = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(secret);
        }

        public string? GenerateQrCodeValue(string? base32Secret, string? username)
        {
            // Validate inputs
            if (username == null || base32Secret == null)
            {
                return null;
            }

            try
            {
                // Generate URI for the QR Code
                var uriString = new OtpUri(OtpType.Totp, base32Secret, username, "Test").ToString();
                return uriString;
            }
            catch
            {
                return null;
            }
        }

        public bool ValidateTotp(string? base32Secret, string totp)
        {
            try
            {
                var secret = Base32Encoding.ToBytes(base32Secret);

                // Get exact time for TOTP, GMT(+7)
                DateTime exactTime = GetNistTime().AddHours(-7);

                // Validate TOTP
                var correction = new TimeCorrection(exactTime);
                var totpValidator = new Totp(secret, timeCorrection: correction);
                bool verify = totpValidator.VerifyTotp(
                    totp,
                    out long timeWindowUsed,
                    VerificationWindow.RfcSpecifiedNetworkDelay
                );

                // Check if TOTP has been used
                if (timeWindowUsedCurrent == timeWindowUsed)
                {
                    return false;
                }
                timeWindowUsedCurrent = timeWindowUsed;

                return verify;
            }
            catch
            {
                return false;
            }
        }

        public static DateTime GetNistTime()
        {
            // Get time from the response header of request to "http://www.google.com"
            using var httpClient = new HttpClient();
            try
            {
                using var response = httpClient.GetAsync("http://www.google.com").Result;
                if (response.IsSuccessStatusCode && response.Headers.Date != null)
                {
                    return DateTime.ParseExact(
                        response.Headers.Date.Value.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'"),
                        "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                        CultureInfo.InvariantCulture.DateTimeFormat,
                        DateTimeStyles.AssumeUniversal
                    );
                }
                else
                {
                    throw new Exception("Failed to get exact time for the TOTP");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get exact time for the TOTP", ex);
            }
        }
    }
}
