namespace HackerNews.BestStories.Api.Clients;

using Microsoft.Extensions.Caching.Hybrid;
using Models;

public sealed class CachingHackerNewsClient : IHackerNewsClient
{
    private const string BestStoryIdsCacheKey = "hacker-news:best-story-ids";
    private const string StoryCacheKeyPrefix = "hacker-news:story:";

    /// <summary>
    /// The full list: could change frequently hence cache it for a short time.
    /// </summary>
    private static readonly HybridCacheEntryOptions BestStoryIdsCacheOptions =
        new()
        {
            Expiration = TimeSpan.FromMinutes(1),
            LocalCacheExpiration = TimeSpan.FromMinutes(1),
        };

    /// <summary>
    /// The stories: tend to be almost readonly hence cache them for a long time.
    /// </summary>
    private static readonly HybridCacheEntryOptions StoryCacheOptions =
        new()
        {
            Expiration = TimeSpan.FromMinutes(15),
            LocalCacheExpiration = TimeSpan.FromMinutes(15),
        };

    private readonly IHackerNewsClient _innerClient;
    private readonly HybridCache _cache;

    public CachingHackerNewsClient(
        IHackerNewsClient innerClient,
        HybridCache cache)
    {
        _innerClient = innerClient;
        _cache = cache;
    }

    public async Task<IReadOnlyList<long>> GetBestStoryIdsAsync(CancellationToken cancellationToken)
    {
        var ids = await _cache.GetOrCreateAsync(
            key: BestStoryIdsCacheKey,
            factory: async ct => (await _innerClient.GetBestStoryIdsAsync(ct)).ToArray(),
            options: BestStoryIdsCacheOptions,
            cancellationToken: cancellationToken);
        return ids;
    }

    public async Task<StoryDto> GetStoryAsync(long storyId, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            $"{StoryCacheKeyPrefix}{storyId}",
            async ct => await _innerClient.GetStoryAsync(storyId, ct),
            StoryCacheOptions,
            cancellationToken: cancellationToken);
    }
}