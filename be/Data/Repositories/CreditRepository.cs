using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories
{
    public class CreditRepository : Repository<UserCredit>, ICreditRepository
    {
        public CreditRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<UserCredit>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserCredit>> GetByUserIdForUpdateAsync(int userId)
        {
            return await _context.UserCredits
                .FromSqlRaw("SELECT * FROM user_credits WHERE user_id = {0} FOR UPDATE", userId)
                .ToListAsync();
        }
    }
}
