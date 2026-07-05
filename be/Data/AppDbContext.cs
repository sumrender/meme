using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<MemeTemplate> MemeTemplates => Set<MemeTemplate>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Album> Albums => Set<Album>();
        public DbSet<Meme> Memes => Set<Meme>();
        public DbSet<UserCredit> UserCredits => Set<UserCredit>();
        public DbSet<CreditTransaction> CreditTransactions => Set<CreditTransaction>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
