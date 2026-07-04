using Backend.Dtos;

namespace Backend.Services
{
    public interface IMemeService
    {
        Task<List<MemeResponseDto>> GetRandomMemes();
        Task<List<MemeResponseDto>> GenerateMemesForContent(string content);
    }
}
