namespace HackerNews.BestStories.Api.Exceptions;

public class ApiResponseException : Exception
{
    public ApiResponseException(string message)
        : base(message)
    {
    }
}