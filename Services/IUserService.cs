using EcommerceWebApi.Entities;
using EcommerceWebApi.Utilities;

namespace EcommerceWebApi.Services
{
    public interface IUserService
    {
        Task<bool> DeleteUserAsync(string id);
        void Dispose();
        List<User> GetAllUsers();
        List<User> GetPaginationUsers(PaginationFilter paginationFilter);
        User? GetUserById(string id);
        User? GetUserByName(string name);
        User? GetUserByToken(string token);
        Task<bool> InsertUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> UpdateUserPropertyAsync<T>(User user, string property, T value);
        Task<bool> UpdateUserTokenAsync(User user, RefreshToken? refreshToken);
    }
}
