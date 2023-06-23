using System.ComponentModel.DataAnnotations;

namespace LoginForm.Models.Request
{
    public class SecondFactorAuthRequest
    {
        public string? UserName { get; set; }

        public string? QrCode { get; set; }

        [Required]
        public string Totp { get; set; } = null!;
    }
}
