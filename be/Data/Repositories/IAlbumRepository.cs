using Backend.Models;

namespace Backend.Data.Repositories
{
    public interface IAlbumRepository : IRepository<Album>
    {
        Task<IEnumerable<Album>> GetByUserIdAsync(int userId);
        Task<Album?> GetByIdWithMemesAsync(int id, int userId);
    }
}
