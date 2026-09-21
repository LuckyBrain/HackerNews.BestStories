using HackerNews.BestStories.Api.Models;
using HackerNews.BestStories.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.BestStories.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class StoriesController : ControllerBase
{
    private readonly IHackerNewsService _hackerNewsService;

    public StoriesController(IHackerNewsService hackerNewsService)
    {
        _hackerNewsService = hackerNewsService;
    }

    [HttpGet("best/wide")]
    public async Task<ActionResult<IReadOnlyList<StoryDto>>> GetBestStoriesAsync(
        [FromQuery] int n,
        CancellationToken cancellationToken = default)
    {
        if (n < 1)
        {
            return BadRequest("n must be greater than zero.");
        }

        var stories = await _hackerNewsService.GetBestStoriesAsync(
            n,
            cancellationToken);

        return Ok(stories);
    }
}