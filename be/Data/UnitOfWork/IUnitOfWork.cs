using Backend.Data.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Backend.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IMemeTemplateRepository MemeTemplates { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IAlbumRepository Albums { get; }
        IMemeRepository Memes { get; }
        ICreditRepository Credits { get; }
        ICreditTransactionRepository CreditTransactions { get; }
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
