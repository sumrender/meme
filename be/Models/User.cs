namespace Backend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Credits { get; set; }
    }
}
