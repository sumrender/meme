namespace Backend.Models
{
    public class CreditTransaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public CreditType CreditType { get; set; }
        public TransactionType TransactionType { get; set; }
        public int Amount { get; set; }
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null!;
    }
}
