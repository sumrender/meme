using Backend.Models;

namespace Backend.Data.Repositories
{
    public interface ICreditRepository : IRepository<UserCredit>
    {
        Task<IEnumerable<UserCredit>> GetByUserIdAsync(int userId);
        Task<IEnumerable<UserCredit>> GetByUserIdForUpdateAsync(int userId);
    }
}
