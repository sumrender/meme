using Backend.Data.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Backend.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IUserRepository? _users;
        private IMemeTemplateRepository? _memeTemplates;
        private IRefreshTokenRepository? _refreshTokens;
        private IAlbumRepository? _albums;
        private IMemeRepository? _memes;
        private ICreditRepository? _credits;
        private ICreditTransactionRepository? _creditTransactions;

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

        public IAlbumRepository Albums =>
            _albums ??= new AlbumRepository(_context);

        public IMemeRepository Memes =>
            _memes ??= new MemeRepository(_context);

        public ICreditRepository Credits =>
            _credits ??= new CreditRepository(_context);

        public ICreditTransactionRepository CreditTransactions =>
            _creditTransactions ??= new CreditTransactionRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
