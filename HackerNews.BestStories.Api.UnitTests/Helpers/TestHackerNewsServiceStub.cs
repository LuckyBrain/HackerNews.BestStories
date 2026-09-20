namespace HackerNews.BestStories.Api.UnitTests.Helpers;

using HackerNews.BestStories.Api.Services;
using Models;

internal sealed class HackerNewsServiceStub : IHackerNewsService
{
    private readonly IReadOnlyList<StoryDto> _stories;

    public HackerNewsServiceStub(IReadOnlyList<StoryDto> stories)
    {
        _stories = stories;
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