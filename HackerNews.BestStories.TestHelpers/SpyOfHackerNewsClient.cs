namespace HackerNews.BestStories.TestHelpers;

using Api.Clients;
using Api.Models;

public sealed class SpyOfHackerNewsClient : IHackerNewsClient
{
    private readonly IReadOnlyList<long> _storyIds;
    private readonly IReadOnlyDictionary<long, StoryDto> _stories;
    private readonly TimeSpan _delay;

    public SpyOfHackerNewsClient(
        IReadOnlyList<long>? storyIds = null,
        IReadOnlyDictionary<long, StoryDto>? stories = null,
        TimeSpan? delay = null)
    {
        _storyIds = storyIds ?? Array.Empty<long>();
        _stories = stories ?? new Dictionary<long, StoryDto>();
        _delay = delay ?? TimeSpan.Zero;
    }

    public int GetBestStoryIdsCallCount { get; private set; }

    public int GetStoryCallCount { get; private set; }

    public async Task<IReadOnlyList<long>> GetBestStoryIdsAsync(CancellationToken cancellationToken)
    {
        GetBestStoryIdsCallCount++;
        if (_delay > TimeSpan.Zero) await Task.Delay(_delay, cancellationToken);

        return _storyIds;
    }

    public async Task<StoryDto> GetStoryAsync(long storyId, CancellationToken cancellationToken)
    {
        GetStoryCallCount++;
        if (_delay > TimeSpan.Zero) await Task.Delay(_delay, cancellationToken);

        return _stories[storyId];
    }
}