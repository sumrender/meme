using Backend.Models;

namespace Backend.Data.Repositories
{
    public class MemeTemplateRepository : Repository<MemeTemplate>, IMemeTemplateRepository
    {
        public MemeTemplateRepository(AppDbContext context) : base(context) { }
    }
}
