namespace HackerNews.BestStories.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

[ApiController]
[Route("api/stories")] // don't use substitution by Reflection: we want this exact route
public sealed class CompactStoriesController : ControllerBase
{
    private readonly ICompactStoriesService _compactStoriesService;

    public CompactStoriesController(ICompactStoriesService compactStoriesService)
    {
        _compactStoriesService = compactStoriesService;
    }

    [HttpGet("best/compact")]
    public async Task<ActionResult<IReadOnlyList<CompactStoryDto>>> GetBestStoriesAsync(
        [FromQuery] int n,
        CancellationToken cancellationToken = default)
    {
        if (n < 1) return BadRequest("n must be greater than zero.");

        var stories = await _compactStoriesService.GetBestStoriesAsync(n, cancellationToken);

        return Ok(stories);
    }
}