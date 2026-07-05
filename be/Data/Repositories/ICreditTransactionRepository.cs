using Backend.Models;

namespace Backend.Data.Repositories
{
    public interface ICreditTransactionRepository : IRepository<CreditTransaction>
    {
        Task<IEnumerable<CreditTransaction>> GetByUserIdAsync(int userId);
    }
}
