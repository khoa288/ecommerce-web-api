using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApi.DTOs
{
    public class TotpDTO
    {
        [Required]
        public string QrCode { get; set; } = null!;

        [Required]
        public string Totp { get; set; } = null!;
    }
}
