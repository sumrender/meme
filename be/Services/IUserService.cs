using Backend.Models;

namespace Backend.Services
{
    public interface IUserService
    {
        Task<User?> GetUserById(int id);
        Task<User?> GetUserByEmail(string email);
        Task<User> CreateUser(string email);
        Task<User> UpdateUserCredits(string email, int newCredits);
    }
}
