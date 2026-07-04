using Backend.Data.Repositories;

namespace Backend.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IMemeTemplateRepository MemeTemplates { get; }
        Task<int> SaveChangesAsync();
    }
}
