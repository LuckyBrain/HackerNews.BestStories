namespace HackerNews.BestStories.Api.ErrorHandling;

using System.Net;
using Exceptions;
using Microsoft.AspNetCore.Diagnostics;

public sealed class HackerNewsExceptionHandler : IExceptionHandler
{
    private readonly ILogger<HackerNewsExceptionHandler> _logger;

    private static readonly Action<ILogger, Exception?> LogApiException =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(1, nameof(HackerNewsExceptionHandler)),
            "Hacker News API request failed.");

    public HackerNewsExceptionHandler(ILogger<HackerNewsExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            ApiUnreachableException => HttpStatusCode.ServiceUnavailable,
            ApiResponseException => HttpStatusCode.BadGateway,
            _ => (HttpStatusCode?)null,
        };

        if (statusCode is null) return false;

        LogApiException(_logger, exception);

        await Results.Problem(
                statusCode: (int)statusCode,
                title: "Hacker News API error",
                detail: exception.Message)
            .ExecuteAsync(httpContext);

        return true;
    }
}