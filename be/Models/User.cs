namespace Backend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string GoogleId { get; set; } = "";
        public string? Name { get; set; }
        public string? PictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<Album> Albums { get; set; } = new List<Album>();
        public ICollection<UserCredit> Credits { get; set; } = new List<UserCredit>();
        public ICollection<CreditTransaction> CreditTransactions { get; set; } = new List<CreditTransaction>();
    }
}
