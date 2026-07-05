namespace Backend.Dtos
{
    public class AlbumListDto
    {
        public int Id { get; set; }
        public string TextSnippet { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public int MemeCount { get; set; }
        public List<string> Thumbnails { get; set; } = new();
    }
}
