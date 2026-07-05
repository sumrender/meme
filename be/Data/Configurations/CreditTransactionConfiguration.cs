using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backend.Models;

namespace Backend.Data.Configurations
{
    public class CreditTransactionConfiguration : IEntityTypeConfiguration<CreditTransaction>
    {
        public void Configure(EntityTypeBuilder<CreditTransaction> builder)
        {
            builder.ToTable("credit_transactions");

            builder.HasKey(ct => ct.Id);

            builder.Property(ct => ct.Id).HasColumnName("id");
            builder.Property(ct => ct.UserId).HasColumnName("user_id").IsRequired();
            builder.Property(ct => ct.CreditType).HasColumnName("credit_type").IsRequired();
            builder.Property(ct => ct.TransactionType).HasColumnName("transaction_type").IsRequired();
            builder.Property(ct => ct.Amount).HasColumnName("amount").IsRequired();
            builder.Property(ct => ct.ReferenceType).HasColumnName("reference_type").HasMaxLength(100);
            builder.Property(ct => ct.ReferenceId).HasColumnName("reference_id");
            builder.Property(ct => ct.CreatedAt).HasColumnName("created_at").IsRequired();

            builder.HasIndex(ct => ct.UserId);

            builder.HasOne(ct => ct.User)
                   .WithMany(u => u.CreditTransactions)
                   .HasForeignKey(ct => ct.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
