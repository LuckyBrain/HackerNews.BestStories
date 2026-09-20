namespace HackerNews.BestStories.Api.Services;

using Clients;
using Models;

public sealed class HackerNewsService : IHackerNewsService
{
    private readonly IHackerNewsClient _hackerNewsClient;

    public HackerNewsService(IHackerNewsClient hackerNewsClient)
    {
        _hackerNewsClient = hackerNewsClient;
    }

    public async Task<IReadOnlyList<StoryDto>> GetBestStoriesAsync(
        int requestedCount,
        CancellationToken cancellationToken)
    {
        if (requestedCount < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(requestedCount),
                requestedCount,
                "Requested count must be greater than zero.");
        }

        var storyIds = await _hackerNewsClient.GetBestStoryIdsAsync(cancellationToken);

        var stories = new List<StoryDto>(storyIds.Count);
        foreach (var storyId in storyIds)
        {
            var story = await _hackerNewsClient.GetStoryAsync(storyId, cancellationToken);
            stories.Add(story);
        }

        return stories
            .OrderByDescending(story => story.Score)
            .Take(requestedCount)
            .ToArray();
    }
}