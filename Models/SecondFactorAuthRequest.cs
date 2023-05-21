using System.ComponentModel.DataAnnotations;

namespace LoginJWT.Models
{
    public class SecondFactorAuthRequest
    {
        public string? UserName { get; set; }

        [Required]
        public string Totp { get; set; } = null!;
    }
}
