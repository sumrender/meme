namespace Backend.Dtos
{
    public class CreditResponseDto
    {
        public int Credits { get; set; }
    }

    public class GenerateMemesResponseDto
    {
        public List<MemeResponseDto> Memes { get; set; } = new();
        public int RemainingCredits { get; set; }
    }
}
