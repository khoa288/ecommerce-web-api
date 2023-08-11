namespace EcommerceWebApi.Authentication
{
    public interface ITotpService
    {
        string GenerateBase32Secret();
        string? GenerateQrCode(string? base32Secret, string? username);
        string? GetSecretFromQrCode(string qrCode);
        bool ValidateTotp(string? base32Secret, string totp);
    }
}
