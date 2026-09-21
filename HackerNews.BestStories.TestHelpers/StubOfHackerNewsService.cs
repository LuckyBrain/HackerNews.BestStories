namespace HackerNews.BestStories.TestHelpers;

using Api.Models;
using Api.Services;

public sealed class StubOfHackerNewsService : IHackerNewsService
{
    private readonly IReadOnlyList<StoryDto> _stories;

    public StubOfHackerNewsService(IReadOnlyList<StoryDto>? stories = null)
    {
        _stories = stories ?? new List<StoryDto>();
    }

    public int CallCount { get; private set; }

    public int? RequestedCount { get; private set; }

    public Task<IReadOnlyList<StoryDto>> GetBestStoriesAsync(
        int requestedCount,
        CancellationToken cancellationToken)
    {
        CallCount++;
        RequestedCount = requestedCount;

        return Task.FromResult(_stories);
    }
}