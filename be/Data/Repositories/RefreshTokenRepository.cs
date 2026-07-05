using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data.Repositories
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context) { }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbSet.Include(r => r.User).FirstOrDefaultAsync(r => r.Token == token);
        }
    }
}
