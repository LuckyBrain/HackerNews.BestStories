using Microsoft.AspNetCore.Mvc;

namespace HackerNews.BestStories.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class StoriesController : ControllerBase
{
    private readonly ILogger<StoriesController> _logger;

    public StoriesController(ILogger<StoriesController> logger)
    {
        _logger = logger;
    }
}