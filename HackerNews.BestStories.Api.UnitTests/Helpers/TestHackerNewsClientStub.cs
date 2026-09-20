namespace HackerNews.BestStories.Api.UnitTests.Helpers;

using HackerNews.BestStories.Api.Clients;
using Models;

internal sealed class HackerNewsClientStub : IHackerNewsClient
{
    private readonly IReadOnlyList<long> _storyIds;
    private readonly IReadOnlyDictionary<long, StoryDto> _stories;

    public HackerNewsClientStub(
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