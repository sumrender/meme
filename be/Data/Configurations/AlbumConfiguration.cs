using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backend.Models;

namespace Backend.Data.Configurations
{
    public class AlbumConfiguration : IEntityTypeConfiguration<Album>
    {
        public void Configure(EntityTypeBuilder<Album> builder)
        {
            builder.ToTable("albums");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.TextContent).HasColumnName("text_content").IsRequired();
            builder.Property(a => a.CreatedAt).HasColumnName("created_at").IsRequired();
            builder.Property(a => a.UpdatedAt).HasColumnName("updated_at").IsRequired();
            builder.Property(a => a.UserId).HasColumnName("user_id").IsRequired();

            builder.HasOne(a => a.User)
                   .WithMany(u => u.Albums)
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
