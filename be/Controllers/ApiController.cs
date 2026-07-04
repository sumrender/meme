using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using Backend.Dtos;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class ApiController : ControllerBase
    {
        private readonly IMemeService _memeService;
        private readonly IUserService _userService;

        public ApiController(IMemeService memeService, IUserService userService)
        {
            _memeService = memeService;
            _userService = userService;
        }

        [HttpPost("auth")]
        public async Task<IActionResult> Auth([FromBody] AuthRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { Message = "Email cannot be empty." });
            }
            var user = await _userService.CreateUser(request.Email);
            var response = new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Credits = user.Credits
            };
            return Ok(response);
        }

        [HttpPost("/generate-memes")]
        [SwaggerOperation(
            Summary = "Generate memes from text content",
            Description = "Creates a list of meme images based on the provided text content.",
            OperationId = "GenerateMemes",
            Tags = new[] { "Meme Operations" }
        )]
        [SwaggerResponse(200, "Returns a list of generated memes as URLs.")]
        [SwaggerResponse(400, "The provided text content is invalid or empty.")]
        [SwaggerResponse(500, "An internal server error occurred while processing the request.")]
        public async Task<IActionResult> GenerateMemes(
            [FromBody, SwaggerParameter("The request body containing the text content for meme generation.", Required = true)]
            GenerateMemesRequestDto request
        )
        {
            if (request == null || string.IsNullOrWhiteSpace(request.TextContent))
            {
                return BadRequest(new { Message = "Text content cannot be empty or whitespace." });
            }
            try
            {
                var memes = await _memeService.GenerateMemesForContent(request.TextContent);
                return Ok(memes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while generating memes.", Details = ex.Message });
            }
        }

        [HttpPost("/generate-random-memes")]
        [SwaggerOperation(
            Summary = "Generate random memes",
            OperationId = "GenerateRandomMemes",
            Tags = new[] { "Meme Operations" }
        )]
        [SwaggerResponse(200, "Returns a list of generated memes as URLs.")]
        [SwaggerResponse(400, "The provided text content is invalid or empty.")]
        [SwaggerResponse(500, "An internal server error occurred while processing the request.")]
        public async Task<IActionResult> GenerateRandomMemes()
        {
            try
            {
                var memes = await _memeService.GetRandomMemes();
                return Ok(memes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while generating memes.", Details = ex.Message });
            }
        }
    }
}
