using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backend.Models;

namespace Backend.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id).HasColumnName("id");
            builder.Property(u => u.Email).HasColumnName("email").IsRequired().HasMaxLength(255);
            builder.Property(u => u.GoogleId).HasColumnName("google_id").IsRequired().HasMaxLength(255);
            builder.Property(u => u.Name).HasColumnName("name").HasMaxLength(255);
            builder.Property(u => u.PictureUrl).HasColumnName("picture_url").HasMaxLength(1024);
            builder.Property(u => u.CreatedAt).HasColumnName("created_at").IsRequired();
            builder.Property(u => u.UpdatedAt).HasColumnName("updated_at").IsRequired();

            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.GoogleId).IsUnique();
        }
    }
}
