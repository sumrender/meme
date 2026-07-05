namespace Backend.Models
{
    public class UserCredit
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public CreditType CreditType { get; set; }
        public int Amount { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User User { get; set; } = null!;
    }
}
