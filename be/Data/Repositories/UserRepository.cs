using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByGoogleIdAsync(string googleId)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.GoogleId == googleId);
        }
    }
}
