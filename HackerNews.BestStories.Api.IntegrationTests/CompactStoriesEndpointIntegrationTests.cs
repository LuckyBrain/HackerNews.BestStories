namespace HackerNews.BestStories.Api.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Models;
using Services;
using TestHelpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public class CompactStoriesEndpointIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CompactStoriesEndpointIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task WhenRequestedCountIsLessThanOne_ShouldReturnBadRequest()
    {
        var service = new StubOfCompactStoriesService();
        await using var factory = CreateFactory(service);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/stories/best/compact?n=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, service.CallCount);
    }

    [Fact]
    public async Task WhenRequestedCountIsValid_ShouldReturnCompactStories()
    {
        const int requestedCount = 2;
        var expected = new[] { DtoFactory.CreateCompactStory(id: 1, score: 500), DtoFactory.CreateCompactStory(id: 2, score: 400) };
        var service = new StubOfCompactStoriesService(expected);
        await using var factory = CreateFactory(service);
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/stories/best/compact?n={requestedCount}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actual = await response.Content.ReadFromJsonAsync<CompactStoryDto[]>();
        Assert.NotNull(actual);
        Assert.Equal(expected, actual);
        Assert.Equal(1, service.CallCount);
        Assert.Equal(requestedCount, service.RequestedCount);
        await AssertCompactJsonContract(response);
    }

    private WebApplicationFactory<Program> CreateFactory(ICompactStoriesService service)
    {
        return _factory.WithWebHostBuilder(
            builder =>
            {
                builder.ConfigureServices(
                    services =>
                    {
                        services.RemoveAll<ICompactStoriesService>();
                        services.AddSingleton(service);
                    });
            });
    }

    private static async Task AssertCompactJsonContract(
        HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        var story = document.RootElement[0];

        Assert.True(story.TryGetProperty("title", out _));
        Assert.True(story.TryGetProperty("uri", out _));
        Assert.True(story.TryGetProperty("postedBy", out _));
        Assert.True(story.TryGetProperty("time", out _));
        Assert.True(story.TryGetProperty("score", out _));
        Assert.True(story.TryGetProperty("commentCount", out _));
        Assert.False(story.TryGetProperty("id", out _));
        Assert.False(story.TryGetProperty("by", out _));
        Assert.False(story.TryGetProperty("descendants", out _));
        Assert.False(story.TryGetProperty("type", out _));
        Assert.False(story.TryGetProperty("url", out _));
    }
}