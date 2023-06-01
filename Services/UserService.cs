using LoginForm.Entities;

namespace LoginForm.Services
{
    public class UserService
    {
        private static readonly List<User> UserList = new();

        public void AddUser(User newUser)
        {
            UserList.Add(newUser);
        }

        public User? GetUser(string? username = null, string? token = null)
        {
            // Get user from username or refresh token
            try
            {
                User? result = null;
                if (!string.IsNullOrEmpty(username))
                {
                    result = UserList.FirstOrDefault(x => x.UserName == username);
                }

                if (!string.IsNullOrEmpty(token))
                {
                    result = UserList.FirstOrDefault(x => x.RefreshToken.Token == token);
                }

                return result;
            }
            catch
            {
                return null;
            }
        }

        public void UpdateUser(User user, RefreshToken? refreshToken)
        {
            // Update user's refresh token
            if (refreshToken != null)
            {
                user.RefreshToken = refreshToken;
            }
            else
            {
                user.RefreshToken.Token = "";
            }
        }

        public void UpdateTwoFactorAuth(User user)
        {
            // Update user's two-factor-auth option
            user.IsTwoFactorAuthActivated = !user.IsTwoFactorAuthActivated;
            if (!user.IsTwoFactorAuthActivated)
            {
                user.SecretCode = null;
            }
        }
    }
}
