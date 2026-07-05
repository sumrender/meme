using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backend.Models;

namespace Backend.Data.Configurations
{
    public class UserCreditConfiguration : IEntityTypeConfiguration<UserCredit>
    {
        public void Configure(EntityTypeBuilder<UserCredit> builder)
        {
            builder.ToTable("user_credits");

            builder.HasKey(uc => uc.Id);

            builder.Property(uc => uc.Id).HasColumnName("id");
            builder.Property(uc => uc.UserId).HasColumnName("user_id").IsRequired();
            builder.Property(uc => uc.CreditType).HasColumnName("credit_type").IsRequired();
            builder.Property(uc => uc.Amount).HasColumnName("amount").IsRequired();
            builder.Property(uc => uc.UpdatedAt).HasColumnName("updated_at").IsRequired();

            builder.HasIndex(uc => new { uc.UserId, uc.CreditType }).IsUnique();

            builder.HasOne(uc => uc.User)
                   .WithMany(u => u.Credits)
                   .HasForeignKey(uc => uc.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
