namespace HackerNews.BestStories.TestHelpers;

using Api.Clients;
using Api.Models;

public sealed class StubOfHackerNewsClient : IHackerNewsClient
{
    private readonly IReadOnlyList<long> _storyIds;
    private readonly IReadOnlyDictionary<long, StoryDto> _stories;

    public StubOfHackerNewsClient(
        IReadOnlyList<long> storyIds,
        IReadOnlyDictionary<long, StoryDto> stories)
    {
        _storyIds = storyIds;
        _stories = stories;
    }

    public Task<IReadOnlyList<long>> GetBestStoryIdsAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_storyIds);
    }

    public Task<StoryDto> GetStoryAsync(
        long storyId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_stories[storyId]);
    }
}