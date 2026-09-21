namespace HackerNews.BestStories.Api.UnitTests.Clients;

using HackerNews.BestStories.Api.Clients;
using Models;
using TestHelpers;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;

public class CachingHackerNewsClientTests
{
    private static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddHybridCache();
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task WhenBestStoryIdsAreRequestedRepeatedly_ShouldCallInnerClientOnce()
    {
        var expected = new long[] { 100, 200, 300 };
        var innerClient = new SpyOfHackerNewsClient(expected);
        await using var serviceProvider = CreateServiceProvider();
        var cache = serviceProvider.GetRequiredService<HybridCache>();
        IHackerNewsClient sut = new CachingHackerNewsClient(innerClient, cache);

        var first = await sut.GetBestStoryIdsAsync();
        var second = await sut.GetBestStoryIdsAsync();

        Assert.Equal(expected, first);
        Assert.Equal(expected, second);
        Assert.Equal(1, innerClient.GetBestStoryIdsCallCount);
    }

    [Fact]
    public async Task WhenStoryIsRequestedRepeatedly_ShouldCallInnerClientOnce()
    {
        const long storyId = 123;
        var expected = DtoFactory.CreateStory(id: storyId);
        var innerClient = new SpyOfHackerNewsClient(
            stories: new Dictionary<long, StoryDto> { [storyId] = expected });
        await using var serviceProvider = CreateServiceProvider();
        var cache = serviceProvider.GetRequiredService<HybridCache>();
        IHackerNewsClient sut = new CachingHackerNewsClient(innerClient, cache);

        var first = await sut.GetStoryAsync(storyId);
        var second = await sut.GetStoryAsync(storyId);

        Assert.Equal(expected, first);
        Assert.Equal(expected, second);
        Assert.Equal(1, innerClient.GetStoryCallCount);
    }

    [Fact]
    public async Task WhenSameStoryIsRequestedConcurrently_ShouldCallInnerClientOnce()
    {
        const long storyId = 123;
        var expected = DtoFactory.CreateStory(id: storyId);
        var innerClient = new SpyOfHackerNewsClient(
            stories: new Dictionary<long, StoryDto> { [storyId] = expected },
            delay: TimeSpan.FromMilliseconds(100));
        await using var serviceProvider = CreateServiceProvider();
        var cache = serviceProvider.GetRequiredService<HybridCache>();
        IHackerNewsClient sut = new CachingHackerNewsClient(innerClient, cache);

        var tasks = Enumerable
            .Range(0, 20)
            .Select(_ => sut.GetStoryAsync(storyId))
            .ToArray();
        var actual = await Task.WhenAll(tasks);

        Assert.All(actual, story => Assert.Equal(expected, story));
        Assert.Equal(1, innerClient.GetStoryCallCount);
    }

    [Fact]
    public async Task WhenDifferentStoriesAreRequested_ShouldCacheEachStorySeparately()
    {
        var story1 = DtoFactory.CreateStory(id: 1);
        var story2 = DtoFactory.CreateStory(id: 2);
        var innerClient = new SpyOfHackerNewsClient(
            stories: new Dictionary<long, StoryDto> { [1] = story1, [2] = story2 });
        await using var serviceProvider = CreateServiceProvider();
        var cache = serviceProvider.GetRequiredService<HybridCache>();
        IHackerNewsClient sut = new CachingHackerNewsClient(innerClient, cache);

        var actual1 = await sut.GetStoryAsync(1);
        var actual2 = await sut.GetStoryAsync(2);

        Assert.Equal(story1, actual1);
        Assert.Equal(story2, actual2);
        Assert.Equal(2, innerClient.GetStoryCallCount);
    }
}