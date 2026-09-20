using HackerNews.BestStories.Api.Exceptions;

namespace HackerNews.BestStories.Api.Clients;

public sealed class HackerNewsClient : IHackerNewsClient
{
    private static readonly Uri BaseUri = new("https://hacker-news.firebaseio.com/");
    private static readonly Uri BestStoriesUri = new(BaseUri, "v0/beststories.json");

    private readonly HttpClient _httpClient;

    public HackerNewsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<long>> GetBestStoryIdsAsync(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync(BestStoriesUri, cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            throw new ApiUnreachableException("The Hacker News API could not be reached.", exception);
        }

        using (response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new ApiResponseException($"The Hacker News API returned HTTP {(int)response.StatusCode}.");
            }

            var ids = await response.Content.ReadFromJsonAsync<long[]>(cancellationToken);

            return ids ?? Array.Empty<long>();
        }
    }
}