namespace HackerNews.BestStories.Api.IntegrationTests;

using Clients;

public class HackerNewsClientTests
{
    [Fact]
    public async Task GetBestStoryIdsAsync_ShouldReturnIds()
    {
        using var httpClient = new HttpClient();
        IHackerNewsClient sut = new HackerNewsClient(httpClient);

        var actual = await sut.GetBestStoryIdsAsync();

        Assert.NotEmpty(actual);
        Assert.All(actual, id => Assert.True(id > 0));
        Assert.Equal(actual.Count, actual.Distinct().Count());
    }

    [Fact]
    public async Task GetStoryAsync_ShouldReturnStory()
    {
        using var httpClient = new HttpClient();
        IHackerNewsClient sut = new HackerNewsClient(httpClient);
        var ids = await sut.GetBestStoryIdsAsync();
        var expectedId = ids.First();

        var actual = await sut.GetStoryAsync(expectedId);

        Assert.NotNull(actual);
        Assert.Equal(expectedId, actual.Id);
        Assert.Equal("story", actual.Type);
        Assert.False(string.IsNullOrWhiteSpace(actual.Title));
        Assert.True(actual.Score >= 0);
    }
}