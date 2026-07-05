namespace Backend.Dtos
{
    public class AlbumDetailDto
    {
        public int Id { get; set; }
        public string TextContent { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public List<MemeResponseDto> Memes { get; set; } = new();
    }
}
