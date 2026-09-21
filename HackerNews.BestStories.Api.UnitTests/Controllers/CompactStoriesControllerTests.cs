namespace HackerNews.BestStories.Api.UnitTests.Controllers;

using HackerNews.BestStories.Api.Controllers;
using TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Models;

public class CompactStoriesControllerTests
{
    [Fact]
    public async Task WhenRequestedCountIsLessThanOne_ShouldReturnBadRequest()
    {
        var service = new StubOfCompactStoriesService();
        var sut = new CompactStoriesController(service);

        var actual = await sut.GetBestStoriesAsync(n: 0);

        Assert.IsType<BadRequestObjectResult>(actual.Result);
        Assert.Equal(0, service.CallCount);
    }

    [Fact]
    public async Task WhenRequestedCountIsValid_ShouldReturnBestStories()
    {
        const int requestedCount = 2;
        var expected = new[] { DtoFactory.CreateCompactStory(id: 1, score: 500), DtoFactory.CreateCompactStory(id: 2, score: 400) };
        var service = new StubOfCompactStoriesService(expected);
        var sut = new CompactStoriesController(service);

        var actual = await sut.GetBestStoriesAsync(n: requestedCount);

        var okResult = Assert.IsType<OkObjectResult>(actual.Result);
        var actualStories = Assert.IsAssignableFrom<IReadOnlyList<CompactStoryDto>>(okResult.Value);
        Assert.Equal(expected, actualStories);
        Assert.Equal(1, service.CallCount);
        Assert.Equal(requestedCount, service.RequestedCount);
    }
}