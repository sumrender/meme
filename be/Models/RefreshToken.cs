namespace Backend.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string JwtId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public bool IsActive => !IsRevoked && !IsUsed && DateTime.UtcNow <= ExpiresAt;
    }
}
