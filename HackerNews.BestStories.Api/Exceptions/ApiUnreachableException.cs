namespace HackerNews.BestStories.Api.Exceptions;

public class ApiUnreachableException : Exception
{
    public ApiUnreachableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}