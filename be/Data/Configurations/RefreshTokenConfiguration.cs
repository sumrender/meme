using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backend.Models;

namespace Backend.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("refresh_tokens");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id).HasColumnName("id");
            builder.Property(r => r.Token).HasColumnName("token").IsRequired();
            builder.Property(r => r.JwtId).HasColumnName("jwt_id").IsRequired();
            builder.Property(r => r.CreatedAt).HasColumnName("created_at").IsRequired();
            builder.Property(r => r.ExpiresAt).HasColumnName("expires_at").IsRequired();
            builder.Property(r => r.IsUsed).HasColumnName("is_used").IsRequired();
            builder.Property(r => r.IsRevoked).HasColumnName("is_revoked").IsRequired();
            builder.Property(r => r.UserId).HasColumnName("user_id").IsRequired();

            builder.HasIndex(r => r.Token).IsUnique();

            builder.HasOne(r => r.User)
                   .WithMany(u => u.RefreshTokens)
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
