namespace HackerNews.BestStories.Api.Clients;

using Models;

public interface IHackerNewsClient
{
    Task<IReadOnlyList<long>> GetBestStoryIdsAsync(CancellationToken cancellationToken = default);

    Task<StoryDto> GetStoryAsync(long storyId, CancellationToken cancellationToken = default);
}