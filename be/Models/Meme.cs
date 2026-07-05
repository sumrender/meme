namespace Backend.Models
{
    public class Meme
    {
        public int Id { get; set; }
        public string Caption { get; set; } = "";
        public string MemeTemplate { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public int AlbumId { get; set; }
        public Album Album { get; set; } = null!;
    }
}
