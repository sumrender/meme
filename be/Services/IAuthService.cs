using Backend.Dtos;

namespace Backend.Services
{
    public interface IAuthService
    {
        Task<(AuthResponseDto Response, string RefreshToken)> AuthenticateWithGoogleAsync(string googleIdToken);
        Task<(AuthResponseDto Response, string RefreshToken)> RefreshTokenAsync(string accessToken, string refreshToken);
        Task<(AuthResponseDto Response, string RefreshToken)> RestoreSessionAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string refreshToken);
    }
}
