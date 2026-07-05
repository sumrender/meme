using Backend.Dtos;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        private void SetRefreshTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7),
                Path = "/api/auth"
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleAuthRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.IdToken))
                return BadRequest(new { Message = "Google ID token is required." });

            try
            {
                var (response, refreshToken) = await _authService.AuthenticateWithGoogleAsync(request.IdToken);
                SetRefreshTokenCookie(refreshToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { Message = "Google authentication failed.", Details = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { Message = "Missing refresh token." });

            var authHeader = Request.Headers.Authorization.ToString();
            var accessToken = authHeader.StartsWith("Bearer ") ? authHeader[7..] : authHeader;

            try
            {
                (AuthResponseDto response, string newRefreshToken) result;

                if (string.IsNullOrEmpty(accessToken))
                {
                    result = await _authService.RestoreSessionAsync(refreshToken);
                }
                else
                {
                    result = await _authService.RefreshTokenAsync(accessToken, refreshToken);
                }

                SetRefreshTokenCookie(result.newRefreshToken);
                return Ok(result.response);
            }
            catch (Exception ex)
            {
                Response.Cookies.Delete("refreshToken");
                return Unauthorized(new { Message = "Token refresh failed.", Details = ex.Message });
            }
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
                await _authService.RevokeTokenAsync(refreshToken);

            Response.Cookies.Delete("refreshToken");
            return Ok(new { Message = "Logged out." });
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var name = User.FindFirstValue("name");
            var picture = User.FindFirstValue("picture");

            return Ok(new UserProfileDto
            {
                Id = int.Parse(userId ?? "0"),
                Email = email ?? "",
                Name = string.IsNullOrEmpty(name) ? null : name,
                PictureUrl = string.IsNullOrEmpty(picture) ? null : picture
            });
        }
    }
}
