using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backend.Models;
using Backend.Data.Seed;

namespace Backend.Data.Configurations
{
    public class MemeTemplateConfiguration : IEntityTypeConfiguration<MemeTemplate>
    {
        public void Configure(EntityTypeBuilder<MemeTemplate> builder)
        {
            builder.ToTable("meme_templates");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id).HasColumnName("id");
            builder.Property(m => m.Name).HasColumnName("name").IsRequired().HasMaxLength(255);
            builder.Property(m => m.Url).HasColumnName("url").IsRequired().HasMaxLength(512);
            builder.Property(m => m.Description).HasColumnName("description").IsRequired();
            builder.Property(m => m.Example).HasColumnName("example").IsRequired();

            builder.HasIndex(m => m.Name).IsUnique();

            builder.HasData(MemeTemplateSeedData.Seed);
        }
    }
}
