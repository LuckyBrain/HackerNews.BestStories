namespace HackerNews.BestStories.TestHelpers;

using Api.Models;
using Api.Services;

public sealed class StubOfCompactStoriesService : ICompactStoriesService
{
    private readonly IReadOnlyList<CompactStoryDto> _stories;

    public StubOfCompactStoriesService(IReadOnlyList<CompactStoryDto>? stories = null)
    {
        _stories = stories ?? Array.Empty<CompactStoryDto>();
    }

    public int CallCount { get; private set; }

    public int? RequestedCount { get; private set; }

    public Task<IReadOnlyList<CompactStoryDto>> GetBestStoriesAsync(
        int requestedCount,
        CancellationToken cancellationToken = default)
    {
        CallCount++;
        RequestedCount = requestedCount;

        return Task.FromResult(_stories);
    }
}