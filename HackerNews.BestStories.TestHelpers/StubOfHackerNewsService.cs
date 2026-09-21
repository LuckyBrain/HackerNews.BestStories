using HackerNews.BestStories.Api.Models;
using HackerNews.BestStories.Api.Services;

namespace HackerNews.BestStories.TestHelpers;

public sealed class StubOfHackerNewsService : IHackerNewsService
{
    private readonly IReadOnlyList<StoryDto> _stories;

    public StubOfHackerNewsService(IReadOnlyList<StoryDto>? stories = null)
    {
        _stories = stories ?? Array.Empty<StoryDto>();
    }

    public int CallCount { get; private set; }

    public int? RequestedCount { get; private set; }

    public Task<IReadOnlyList<StoryDto>> GetBestStoriesAsync(
        int requestedCount,
        CancellationToken cancellationToken = default)
    {
        CallCount++;
        RequestedCount = requestedCount;

        return Task.FromResult(_stories);
    }
}