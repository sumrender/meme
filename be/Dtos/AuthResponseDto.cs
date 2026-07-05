namespace Backend.Dtos
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = "";
        public UserProfileDto User { get; set; } = null!;
    }

    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string? Name { get; set; }
        public string? PictureUrl { get; set; }
    }
}
