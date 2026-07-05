using Backend.Data.UnitOfWork;
using Backend.Models;

namespace Backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICreditService _creditService;

        public UserService(IUnitOfWork unitOfWork, ICreditService creditService)
        {
            _unitOfWork = unitOfWork;
            _creditService = creditService;
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _unitOfWork.Users.GetByIdAsync(id);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _unitOfWork.Users.GetByEmailAsync(email);
        }

        public async Task<User> UpsertGoogleUser(string googleId, string email, string? name, string? pictureUrl)
        {
            var user = await _unitOfWork.Users.GetByGoogleIdAsync(googleId);

            if (user != null)
            {
                user.Email = email;
                user.Name = name;
                user.PictureUrl = pictureUrl;
                user.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();
                return user;
            }

            user = new User
            {
                GoogleId = googleId,
                Email = email,
                Name = name,
                PictureUrl = pictureUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            await _creditService.GrantInitialCreditsAsync(user.Id);
            return user;
        }
    }
}
