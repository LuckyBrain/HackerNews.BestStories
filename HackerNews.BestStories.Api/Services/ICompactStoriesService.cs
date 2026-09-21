namespace HackerNews.BestStories.Api.Services;

using Models;

public interface ICompactStoriesService
{
    Task<IReadOnlyList<CompactStoryDto>> GetBestStoriesAsync(
        int requestedCount,
        CancellationToken cancellationToken = default);
}