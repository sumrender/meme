using Backend.Data.UnitOfWork;
using Backend.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/albums")]
    [Authorize]
    [Produces("application/json")]
    public class AlbumsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AlbumsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        [HttpGet]
        public async Task<IActionResult> GetAlbums()
        {
            var userId = GetUserId();
            var albums = await _unitOfWork.Albums.GetByUserIdAsync(userId);

            var result = albums.Select(a => new AlbumListDto
            {
                Id = a.Id,
                TextSnippet = a.TextContent.Length > 100
                    ? a.TextContent[..100] + "..."
                    : a.TextContent,
                CreatedAt = a.CreatedAt,
                MemeCount = a.Memes.Count,
                Thumbnails = a.Memes
                    .Take(4)
                    .Select(m => m.MemeTemplate)
                    .ToList()
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlbum(int id)
        {
            var userId = GetUserId();
            var album = await _unitOfWork.Albums.GetByIdWithMemesAsync(id, userId);

            if (album == null)
                return NotFound();

            var result = new AlbumDetailDto
            {
                Id = album.Id,
                TextContent = album.TextContent,
                CreatedAt = album.CreatedAt,
                Memes = album.Memes.Select(m => new MemeResponseDto
                {
                    Caption = m.Caption,
                    MemeTemplate = m.MemeTemplate
                }).ToList()
            };

            return Ok(result);
        }
    }
}
