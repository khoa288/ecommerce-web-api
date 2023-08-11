using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApi.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords don't match.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
