using Backend.Data.UnitOfWork;
using Backend.Models;

namespace Backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _unitOfWork.Users.GetByIdAsync(id);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _unitOfWork.Users.GetByEmailAsync(email);
        }

        public async Task<User> CreateUser(string email)
        {
            var user = await GetUserByEmail(email);
            if (user != null)
            {
                return user;
            }

            user = new User
            {
                Email = email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Credits = 5
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserCredits(string email, int newCredits)
        {
            var user = await GetUserByEmail(email);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.Credits = newCredits;
            user.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return user;
        }
    }
}
