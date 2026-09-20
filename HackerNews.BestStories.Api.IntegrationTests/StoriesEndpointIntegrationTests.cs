namespace HackerNews.BestStories.Api.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Models;
using Services;

public class StoriesEndpointIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public StoriesEndpointIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task WhenRequestedCountIsLessThanOne_ShouldReturnBadRequest()
    {
        var service = new HackerNewsServiceStub(new List<StoryDto>());

        using var client = CreateClient(service);

        var response = await client.GetAsync("/api/stories/best?n=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, service.CallCount);
    }

    [Fact]
    public async Task WhenRequestedCountIsValid_ShouldReturnBestStories()
    {
        const int requestedCount = 2;
        var expected = new[] { CreateStory(1, 500), CreateStory(2, 400) };

        var service = new HackerNewsServiceStub(expected);

        using var client = CreateClient(service);

        var response = await client.GetAsync($"/api/stories/best?n={requestedCount}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var actual = await response.Content.ReadFromJsonAsync<StoryDto[]>();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual);
        Assert.Equal(1, service.CallCount);
        Assert.Equal(requestedCount, service.RequestedCount);
    }

    private HttpClient CreateClient(
        IHackerNewsService hackerNewsService)
    {
        var factory = _factory.WithWebHostBuilder(
            builder =>
            {
                builder.ConfigureServices(
                    services =>
                    {
                        services.RemoveAll<IHackerNewsService>();
                        services.AddSingleton(hackerNewsService);
                    });
            });

        return factory.CreateClient();
    }

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

    private sealed class HackerNewsServiceStub
        : IHackerNewsService
    {
        private readonly IReadOnlyList<StoryDto> _stories;

        public HackerNewsServiceStub(
            IReadOnlyList<StoryDto> stories)
        {
            _stories = stories;
        }

        public int CallCount { get; private set; }

        public int? RequestedCount { get; private set; }

        public Task<IReadOnlyList<StoryDto>> GetBestStoriesAsync(
            int requestedCount,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            RequestedCount = requestedCount;

            return Task.FromResult(_stories);
        }
    }
}