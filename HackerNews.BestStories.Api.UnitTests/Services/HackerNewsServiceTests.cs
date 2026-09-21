namespace HackerNews.BestStories.Api.UnitTests.Services;

using HackerNews.BestStories.Api.Clients;
using Models;
using HackerNews.BestStories.Api.Services;
using TestHelpers;

public class HackerNewsServiceTests
{
    private static readonly IReadOnlyList<long> StoryIds = new[] { 1L, 2, 3, 4 };

    private static readonly IReadOnlyDictionary<long, StoryDto> Stories = new Dictionary<long, StoryDto>
    {
        [1] = DtoFactory.CreateStory(1, 100),
        [2] = DtoFactory.CreateStory(2, 500),
        [3] = DtoFactory.CreateStory(3, 200),
        [4] = DtoFactory.CreateStory(4, 400),
    };

    private static IHackerNewsService CreateSut()
    {
        IHackerNewsClient client = new StubOfHackerNewsClient(StoryIds, Stories);
        return new HackerNewsService(client);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task WhenRequestedCountIsLessThanOne_ShouldThrow(int requestedCount)
    {
        var sut = CreateSut();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.GetBestStoriesAsync(requestedCount));
    }

    [Fact]
    public async Task WhenStoriesAreReturned_ShouldReturnTopCountOrderedByScore()
    {
        var sut = CreateSut();
        var expected = new[] { DtoFactory.CreateStory(2, 500), DtoFactory.CreateStory(4, 400) };

        var actual = await sut.GetBestStoriesAsync(requestedCount: 2);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task WhenRequestedCountExceedsAvailableStories_ShouldReturnAllStoriesOrderedByScore()
    {
        var sut = CreateSut();
        var expected = new[] { DtoFactory.CreateStory(2, 500), DtoFactory.CreateStory(4, 400), DtoFactory.CreateStory(3, 200), DtoFactory.CreateStory(1, 100) };

        var actual = await sut.GetBestStoriesAsync(requestedCount: 10);

        Assert.Equal(expected, actual);
    }
}