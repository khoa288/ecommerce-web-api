using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApi.Entities
{
    public class User
    {
        [Required(ErrorMessage = "{0} is required")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "{0} is required")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "{0} is required")]
        public string Role = null!;

        public bool IsTwoFactorAuthActivated { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public byte[] PasswordSalt { get; set; } = null!;

        [Required(ErrorMessage = "{0} is required")]
        public byte[] PasswordHash { get; set; } = null!;

        public string? SecretCode { get; set; }

        public RefreshToken RefreshToken { get; set; } = null!;
    }

    public class RefreshToken
    {
        [Required(ErrorMessage = "{0} is required")]
        public string Token { get; set; } = null!;

        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
    }
}
