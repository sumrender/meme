using Backend.Models;

namespace Backend.Services
{
    public interface IUserService
    {
        Task<User?> GetUserById(int id);
        Task<User?> GetUserByEmail(string email);
        Task<User> UpsertGoogleUser(string googleId, string email, string? name, string? pictureUrl);
    }
}
