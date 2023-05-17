namespace LoginJWT.Models
{

    public enum RefreshTokenResult
    {
        Success = 0,
        UserNotExist = -2,
        TokenExpire = -3,
        Other = -1
    }

    public class RefreshTokenResponse
    {
        public RefreshTokenResult ResultCode;
        public User? User;
    }
}
