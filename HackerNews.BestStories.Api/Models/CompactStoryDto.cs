namespace HackerNews.BestStories.Api.Models;

using System.Text.Json.Serialization;

public sealed record CompactStoryDto(
    [property: JsonPropertyName("title")] string? Title,
    [property: JsonPropertyName("uri")] string? Uri,
    [property: JsonPropertyName("postedBy")] string? PostedBy,
    [property: JsonPropertyName("time")] DateTimeOffset Time,
    [property: JsonPropertyName("score")] int Score,
    [property: JsonPropertyName("commentCount")] int CommentCount);