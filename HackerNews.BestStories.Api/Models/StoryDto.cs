namespace HackerNews.BestStories.Api.Models;

public sealed record StoryDto(
    long Id,
    string? By,
    int Descendants,
    int Score,
    long Time,
    string? Title,
    string? Type,
    string? Url);