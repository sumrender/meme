namespace Backend.Models
{
    public class MemeTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Url { get; set; } = "";
        public string Description { get; set; } = "";
        public string Example { get; set; } = "";
    }
}
