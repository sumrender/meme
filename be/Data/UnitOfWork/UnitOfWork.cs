using Backend.Data.Repositories;

namespace Backend.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IUserRepository? _users;
        private IMemeTemplateRepository? _memeTemplates;
        private IRefreshTokenRepository? _refreshTokens;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users =>
            _users ??= new UserRepository(_context);

        public IMemeTemplateRepository MemeTemplates =>
            _memeTemplates ??= new MemeTemplateRepository(_context);

        public IRefreshTokenRepository RefreshTokens =>
            _refreshTokens ??= new RefreshTokenRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
