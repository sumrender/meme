using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Backend.Data.UnitOfWork;
using Backend.Dtos;
using Backend.Models;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;
        private readonly string _googleClientId;

        public AuthService(
            IUserService userService,
            IUnitOfWork unitOfWork,
            IOptions<JwtSettings> jwtSettings,
            IConfiguration configuration)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtSettings.Value;
            _googleClientId = configuration["GOOGLE_CLIENT_ID"] ?? "";
        }

        public async Task<(AuthResponseDto Response, string RefreshToken)> AuthenticateWithGoogleAsync(string googleIdToken)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(googleIdToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _googleClientId }
            });

            var user = await _userService.UpsertGoogleUser(
                payload.Subject,
                payload.Email,
                payload.Name,
                payload.Picture);

            return await GenerateAuthResponse(user);
        }

        public async Task<(AuthResponseDto Response, string RefreshToken)> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
                throw new SecurityTokenException("Invalid access token");

            var jti = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);
            if (string.IsNullOrEmpty(jti))
                throw new SecurityTokenException("Access token missing jti claim");

            var storedRefreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);
            if (storedRefreshToken == null || !storedRefreshToken.IsActive || storedRefreshToken.JwtId != jti)
                throw new SecurityTokenException("Invalid or expired refresh token");

            storedRefreshToken.IsRevoked = true;
            _unitOfWork.RefreshTokens.Update(storedRefreshToken);
            await _unitOfWork.SaveChangesAsync();

            return await GenerateAuthResponse(storedRefreshToken.User);
        }

        public async Task<(AuthResponseDto Response, string RefreshToken)> RestoreSessionAsync(string refreshToken)
        {
            var storedRefreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);
            if (storedRefreshToken == null || !storedRefreshToken.IsActive)
                throw new SecurityTokenException("Invalid or expired refresh token");

            storedRefreshToken.IsRevoked = true;
            _unitOfWork.RefreshTokens.Update(storedRefreshToken);
            await _unitOfWork.SaveChangesAsync();

            return await GenerateAuthResponse(storedRefreshToken.User);
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            var storedToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);
            if (storedToken == null || !storedToken.IsActive)
                return false;

            storedToken.IsRevoked = true;
            _unitOfWork.RefreshTokens.Update(storedToken);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private async Task<(AuthResponseDto Response, string RefreshToken)> GenerateAuthResponse(User user)
        {
            var (accessToken, jti) = GenerateAccessToken(user);
            string refreshToken = GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                JwtId = jti,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshExpirationDays)
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            var response = new AuthResponseDto
            {
                AccessToken = accessToken,
                User = new UserProfileDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    PictureUrl = user.PictureUrl
                }
            };

            return (response, refreshToken);
        }

        private (string Token, string Jti) GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jti = Guid.NewGuid().ToString();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim("name", user.Name ?? ""),
                new Claim("picture", user.PictureUrl ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: credentials
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), jti);
        }

        private static string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;

            return principal;
        }
    }
}
