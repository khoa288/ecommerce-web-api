using LoginJWT.Models;

namespace LoginJWT.Services
{
    public class UserService
    {
        private static readonly List<User> UserList = new();
        public void AddUser(User newUser)
        {
            try
            {
                UserList.Add(newUser);
            }
            catch
            {
                throw new Exception();
            }
        }

        public User? GetUser(string? username = null, string? token = null)
        {
            try
            {
                User? result = null;
                if (!string.IsNullOrEmpty(username))
                {
                    result = UserList.FirstOrDefault(x => x.UserName == username);
                }

                if (!string.IsNullOrEmpty(token))
                {
                    result = UserList.FirstOrDefault(x => x.Token == token);
                }

                return result;

            }
            catch
            {
                return null;
            }
        }

        public void UpdateUser(User user, string? token = null, DateTime? created = null, DateTime? expires = null)
        {
            try
            {
                if (token != null)
                {
                    user.Token = token;
                }

                if (created != null)
                {
                    user.TokenCreated = created.Value;
                }

                if (expires != null)
                {
                    user.TokenExpires = expires.Value;
                }
            }
            catch
            {
                throw new Exception();
            }
        }

        public void UpdateTwoFactorAuth(User user)
        {
            try
            {
                user.IsTwoFactorAuthActivated = !user.IsTwoFactorAuthActivated;
                if (!user.IsTwoFactorAuthActivated)
                {
                    user.SecretCode = "";
                }
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
