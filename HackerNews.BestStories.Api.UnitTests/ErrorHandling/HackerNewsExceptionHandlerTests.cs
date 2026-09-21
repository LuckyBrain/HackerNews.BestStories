namespace HackerNews.BestStories.Api.UnitTests.ErrorHandling;

using System.Net;
using System.Text.Json;
using Exceptions;
using HackerNews.BestStories.Api.ErrorHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

public class HackerNewsExceptionHandlerTests
{
    [Fact]
    public async Task WhenApiIsUnreachable_ShouldReturnServiceUnavailable()
    {
        var expectedException = new ApiUnreachableException("The Hacker News API could not be reached.", new HttpRequestException());

        await AssertHandledAsync(expectedException, HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task WhenApiReturnsError_ShouldReturnBadGateway()
    {
        var expectedException = new ApiResponseException("The Hacker News API returned an error.");

        await AssertHandledAsync(expectedException, HttpStatusCode.BadGateway);
    }

    [Fact]
    public async Task WhenExceptionIsNotHandled_ShouldReturnFalse()
    {
        await using var serviceProvider = CreateServiceProvider();
        var httpContext = CreateHttpContext(serviceProvider);
        var sut = new HackerNewsExceptionHandler(NullLogger<HackerNewsExceptionHandler>.Instance);

        var actual = await sut.TryHandleAsync(
            httpContext,
            new InvalidOperationException("Test"),
            CancellationToken.None);

        Assert.False(actual);
        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal(0, httpContext.Response.Body.Length);
    }

    private static async Task AssertHandledAsync(
        Exception exception,
        HttpStatusCode expectedStatusCode)
    {
        await using var serviceProvider = CreateServiceProvider();
        var httpContext = CreateHttpContext(serviceProvider);

        var sut = new HackerNewsExceptionHandler(NullLogger<HackerNewsExceptionHandler>.Instance);

        var actual = await sut.TryHandleAsync(
            httpContext,
            exception,
            CancellationToken.None);

        Assert.True(actual);
        Assert.Equal((int)expectedStatusCode, httpContext.Response.StatusCode);
        httpContext.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(httpContext.Response.Body);
        var root = document.RootElement;
        Assert.Equal("Hacker News API error", root.GetProperty("title").GetString());
        Assert.Equal((int)expectedStatusCode, root.GetProperty("status").GetInt32());
        Assert.Equal(exception.Message, root.GetProperty("detail").GetString());
    }

    private static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection()
            .AddLogging()
            .AddProblemDetails();

        return services.BuildServiceProvider();
    }

    private static DefaultHttpContext CreateHttpContext(IServiceProvider serviceProvider)
    {
        return new DefaultHttpContext
        {
            RequestServices = serviceProvider,
            Response =
            {
                Body = new MemoryStream()
            }
        };
    }
}