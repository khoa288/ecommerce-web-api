using LoginJWT.Models;

namespace LoginJWT.Services
{
    public class UserService
    {
        private static List<User> UserList = new List<User>();
        public int AddUser(User newUser)
        {
            try
            {
                UserList.Add(newUser);
                return 0;
            }
            catch (Exception ex)
            {
                return -1;
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

        public int UpdateUser(User user, string? token = null, DateTime? created = null, DateTime? expires = null)
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

                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public void UpdateTwoFactorAuth(User user)
        {
            user.IsTwoFactorAuthActivated = !user.IsTwoFactorAuthActivated;
        }
    }
}
