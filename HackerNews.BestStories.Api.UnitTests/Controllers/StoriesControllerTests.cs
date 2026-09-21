namespace HackerNews.BestStories.Api.UnitTests.Controllers;

using HackerNews.BestStories.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Models;
using TestHelpers;

public class StoriesControllerTests
{
    [Fact]
    public async Task WhenRequestedCountIsLessThanOne_ShouldReturnBadRequest()
    {
        var service = new StubOfHackerNewsService();
        var sut = new StoriesController(service);

        var actual = await sut.GetBestStoriesAsync(n: 0);

        Assert.IsType<BadRequestObjectResult>(actual.Result);
        Assert.Equal(0, service.CallCount);
    }

    [Fact]
    public async Task WhenRequestedCountIsValid_ShouldReturnBestStories()
    {
        const int requestedCount = 2;
        var expected = new[] { DtoFactory.CreateStory(1, 500), DtoFactory.CreateStory(2, 400) };
        var service = new StubOfHackerNewsService(expected);
        var sut = new StoriesController(service);

        var actual = await sut.GetBestStoriesAsync(n: requestedCount);

        var okResult = Assert.IsType<OkObjectResult>(actual.Result);
        var actualStories = Assert.IsAssignableFrom<IReadOnlyList<StoryDto>>(okResult.Value);
        Assert.Equal(expected, actualStories);
        Assert.Equal(1, service.CallCount);
        Assert.Equal(requestedCount, service.RequestedCount);
    }
}