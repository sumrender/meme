namespace Backend.Services
{
    public interface IAiService
    {
        Task<string> AskAsync(string input, CancellationToken cancellationToken = default);
    }
}