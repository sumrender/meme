using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories
{
    public class CreditTransactionRepository : Repository<CreditTransaction>, ICreditTransactionRepository
    {
        public CreditTransactionRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CreditTransaction>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(ct => ct.UserId == userId)
                .OrderByDescending(ct => ct.CreatedAt)
                .ToListAsync();
        }
    }
}
