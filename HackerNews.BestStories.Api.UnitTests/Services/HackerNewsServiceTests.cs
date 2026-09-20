namespace HackerNews.BestStories.Api.UnitTests.Services;

using HackerNews.BestStories.Api.Clients;
using Models;
using HackerNews.BestStories.Api.Services;
using Helpers;

public class HackerNewsServiceTests
{
    private static readonly IReadOnlyList<long> StoryIds = new[] { 1L, 2, 3, 4 };

    private static readonly IReadOnlyDictionary<long, StoryDto> Stories = new Dictionary<long, StoryDto>
    {
        [1] = CreateStory(1, 100),
        [2] = CreateStory(2, 500),
        [3] = CreateStory(3, 200),
        [4] = CreateStory(4, 400),
    };

    private static StoryDto CreateStory(long id, int score)
    {
        return new StoryDto(
            id,
            "test-user",
            10,
            score,
            1758390000,
            $"Story {id}",
            "story",
            $"https://example.com/{id}");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task WhenRequestedCountIsLessThanOne_ShouldThrow(int requestedCount)
    {
        IHackerNewsClient client = new HackerNewsClientStub(StoryIds, Stories);
        IHackerNewsService sut = new HackerNewsService(client);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.GetBestStoriesAsync(requestedCount));
    }

    [Fact]
    public async Task WhenStoriesAreReturned_ShouldReturnTopCountOrderedByScore()
    {
        IHackerNewsClient client = new HackerNewsClientStub(StoryIds, Stories);
        IHackerNewsService sut = new HackerNewsService(client);
        var expected = new[] { Stories[2], Stories[4] };

        var actual = await sut.GetBestStoriesAsync(requestedCount: 2);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task WhenRequestedCountExceedsAvailableStories_ShouldReturnAllStoriesOrderedByScore()
    {
        IHackerNewsClient client = new HackerNewsClientStub(StoryIds, Stories);
        IHackerNewsService sut = new HackerNewsService(client);
        var expected = new[] { Stories[2], Stories[4], Stories[3], Stories[1] };

        var actual = await sut.GetBestStoriesAsync(requestedCount: 10);

        Assert.Equal(expected, actual);
    }
}