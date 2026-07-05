using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backend.Models;

namespace Backend.Data.Configurations
{
    public class MemeConfiguration : IEntityTypeConfiguration<Meme>
    {
        public void Configure(EntityTypeBuilder<Meme> builder)
        {
            builder.ToTable("memes");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id).HasColumnName("id");
            builder.Property(m => m.Caption).HasColumnName("caption").IsRequired();
            builder.Property(m => m.MemeTemplate).HasColumnName("meme_template").IsRequired().HasMaxLength(512);
            builder.Property(m => m.CreatedAt).HasColumnName("created_at").IsRequired();
            builder.Property(m => m.AlbumId).HasColumnName("album_id").IsRequired();

            builder.HasOne(m => m.Album)
                   .WithMany(a => a.Memes)
                   .HasForeignKey(m => m.AlbumId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
