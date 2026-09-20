namespace HackerNews.BestStories.Api.Services;

using Models;

public interface IHackerNewsService
{
    Task<IReadOnlyList<StoryDto>> GetBestStoriesAsync(
        int requestedCount,
        CancellationToken cancellationToken = default);
}