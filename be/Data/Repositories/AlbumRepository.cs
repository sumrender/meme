using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories
{
    public class AlbumRepository : Repository<Album>, IAlbumRepository
    {
        public AlbumRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Album>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(a => a.UserId == userId)
                .Include(a => a.Memes)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Album?> GetByIdWithMemesAsync(int id, int userId)
        {
            return await _dbSet
                .Where(a => a.Id == id && a.UserId == userId)
                .Include(a => a.Memes)
                .FirstOrDefaultAsync();
        }
    }
}
