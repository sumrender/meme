using Backend.Data.Repositories;

namespace Backend.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IMemeTemplateRepository MemeTemplates { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IAlbumRepository Albums { get; }
        IMemeRepository Memes { get; }
        Task<int> SaveChangesAsync();
    }
}
