namespace HackerNews.BestStories.Api.Clients;

public interface IHackerNewsClient
{
    Task<IReadOnlyList<long>> GetBestStoryIdsAsync(CancellationToken cancellationToken = default);
}
