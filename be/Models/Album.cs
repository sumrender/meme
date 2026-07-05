namespace Backend.Models
{
    public class Album
    {
        public int Id { get; set; }
        public string TextContent { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<Meme> Memes { get; set; } = new List<Meme>();
    }
}
