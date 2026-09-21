namespace HackerNews.BestStories.Api.Services;

using Adapters;
using Models;

public sealed class CompactStoriesService : ICompactStoriesService
{
    private readonly IHackerNewsService _hackerNewsService;
    private readonly IStoryAdapter _storyAdapter;

    public CompactStoriesService(
        IHackerNewsService hackerNewsService,
        IStoryAdapter storyAdapter)
    {
        _hackerNewsService = hackerNewsService;
        _storyAdapter = storyAdapter;
    }

    public async Task<IReadOnlyList<CompactStoryDto>> GetBestStoriesAsync(
        int requestedCount,
        CancellationToken cancellationToken = default)
    {
        var stories = await _hackerNewsService.GetBestStoriesAsync(
            requestedCount,
            cancellationToken);

        return stories
            .Select(_storyAdapter.Adapt)
            .ToArray();
    }
}