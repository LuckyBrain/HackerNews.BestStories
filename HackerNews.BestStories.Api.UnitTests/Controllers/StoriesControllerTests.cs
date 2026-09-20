namespace HackerNews.BestStories.Api.UnitTests.Controllers;

using HackerNews.BestStories.Api.Controllers;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Models;

public class StoriesControllerTests
{
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

    [Fact]
    public async Task WhenRequestedCountIsLessThanOne_ShouldReturnBadRequest()
    {
        var service = new HackerNewsServiceStub(new List<StoryDto>());
        var sut = new StoriesController(service);

        var actual = await sut.GetBestStoriesAsync(n: 0);

        Assert.IsType<BadRequestObjectResult>(actual.Result);
        Assert.Equal(0, service.CallCount);
    }

    [Fact]
    public async Task WhenRequestedCountIsValid_ShouldReturnBestStories()
    {
        const int requestedCount = 2;
        var expected = new[] { CreateStory(1, 500), CreateStory(2, 400) };
        var service = new HackerNewsServiceStub(expected);
        var sut = new StoriesController(service);

        var actual = await sut.GetBestStoriesAsync(n: requestedCount);

        var okResult = Assert.IsType<OkObjectResult>(actual.Result);
        var actualStories = Assert.IsAssignableFrom<IReadOnlyList<StoryDto>>(okResult.Value);
        Assert.Equal(expected, actualStories);
        Assert.Equal(1, service.CallCount);
        Assert.Equal(requestedCount, service.RequestedCount);
    }
}