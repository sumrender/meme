using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using System.Security.Claims;
using Backend.Dtos;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class ApiController : ControllerBase
    {
        private readonly IMemeService _memeService;
        private readonly ICreditService _creditService;

        public ApiController(IMemeService memeService, ICreditService creditService)
        {
            _memeService = memeService;
            _creditService = creditService;
        }

        [HttpPost("/generate-memes")]
        [Authorize]
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
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

                var (success, remainingCredits) = await _creditService.TryDeductCreditAsync(userId);
                if (!success)
                {
                    return StatusCode(402, new { Message = "Insufficient credits.", RemainingCredits = 0 });
                }

                try
                {
                    var memes = await _memeService.GenerateMemesForContent(request.TextContent, userId);
                    return Ok(new GenerateMemesResponseDto { Memes = memes, RemainingCredits = remainingCredits });
                }
                catch (Exception)
                {
                    await _creditService.RefundCreditAsync(userId);
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while generating memes.", Details = ex.Message });
            }
        }

        [HttpPost("/generate-random-memes")]
        [Authorize]
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
