using Backend.Models;

namespace Backend.Data.Repositories
{
    public class MemeRepository : Repository<Meme>, IMemeRepository
    {
        public MemeRepository(AppDbContext context) : base(context) { }
    }
}
