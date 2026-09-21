namespace HackerNews.BestStories.Api.UnitTests.Services;

using HackerNews.BestStories.Api.Adapters;
using HackerNews.BestStories.Api.Services;
using TestHelpers;

public class CompactStoriesServiceTests
{
    [Fact]
    public async Task WhenStoriesAreReturned_ShouldReturnAdaptedStories()
    {
        const int requestedCount = 2;
        var stories = new[] { DtoFactory.CreateStory(id: 1, score: 500), DtoFactory.CreateStory(id: 2, score: 400) };
        var hackerNewsService = new StubOfHackerNewsService(stories);
        IStoryAdapter storyAdapter = new StubOfStoryAdapter();
        ICompactStoriesService sut = new CompactStoriesService(hackerNewsService, storyAdapter);
        var expected = new[] { DtoFactory.CreateCompactStory(id: 1, score: 500), DtoFactory.CreateCompactStory(id: 2, score: 400) };

        var actual = await sut.GetBestStoriesAsync(requestedCount);

        Assert.Equal(expected, actual);
        Assert.Equal(1, hackerNewsService.CallCount);
        Assert.Equal(requestedCount, hackerNewsService.RequestedCount);
    }

    [Fact]
    public async Task WhenNoStoriesAreReturned_ShouldReturnEmpty()
    {
        var hackerNewsService = new StubOfHackerNewsService();
        IStoryAdapter storyAdapter = new StubOfStoryAdapter();
        ICompactStoriesService sut = new CompactStoriesService(hackerNewsService, storyAdapter);

        var actual = await sut.GetBestStoriesAsync(10);

        Assert.Empty(actual);
    }
}