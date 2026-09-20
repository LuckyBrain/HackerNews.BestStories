namespace HackerNews.BestStories.Api.IntegrationTests;

using Clients;

public class HackerNewsClientTests
{
    [Fact]
    public async Task GetBestStoryIdsAsync_ShouldReturnIds()
    {
        using var httpClient = new HttpClient();
        var sut = new HackerNewsClient(httpClient);

        var actual = await sut.GetBestStoryIdsAsync();

        Assert.NotEmpty(actual);
        Assert.All(actual, id => Assert.True(id > 0));
        Assert.Equal(actual.Count, actual.Distinct().Count());
    }
}