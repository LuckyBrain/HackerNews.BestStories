using HackerNews.BestStories.Api.Exceptions;

namespace HackerNews.BestStories.Api.Clients;

using Models;

public sealed class HackerNewsClient : IHackerNewsClient
{
    private static readonly Uri BaseUri = new("https://hacker-news.firebaseio.com/");
    private static readonly Uri BestStoriesUri = new(BaseUri, "v0/beststories.json");

    private readonly HttpClient _httpClient;

    public HackerNewsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private static ApiUnreachableException CreateDefaultApiUnreachableException(Exception exception)
    {
        return new ApiUnreachableException("The Hacker News API could not be reached.", exception);
    }

    private static ApiResponseException CreateDefaultApiResponseException(HttpResponseMessage response)
    {
        return new ApiResponseException($"The Hacker News API returned HTTP {(int)response.StatusCode}.");
    }

    private async Task<T?> GetAsync<T>(
        Uri uri,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync(uri, cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            throw CreateDefaultApiUnreachableException(exception);
        }

        using (response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw CreateDefaultApiResponseException(response);
            }

            return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
        }
    }

    public async Task<IReadOnlyList<long>> GetBestStoryIdsAsync(CancellationToken cancellationToken)
    {
        var ids = await GetAsync<long[]>(BestStoriesUri, cancellationToken);
        return ids ?? Array.Empty<long>();
    }

    public async Task<StoryDto> GetStoryAsync(long id, CancellationToken cancellationToken)
    {
        var uri = new Uri(BaseUri, $"v0/item/{id}.json");
        var story = await GetAsync<StoryDto>(uri, cancellationToken);
        return story ?? throw new ApiResponseException($"The Hacker News API returned no story for ID {id}.");
    }
}