namespace EcommerceWebApi.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Role = null!;

        public bool IsTwoFactorAuthActivated { get; set; }
    }
}
