namespace HackerNews.BestStories.TestHelpers;

using Api.Models;

public class DtoFactory
{
    private static readonly DateTimeOffset _storyDateTimeOffset = new(2019, 10, 12, 13, 43, 1, TimeSpan.Zero);
    private static readonly long _unixStoryTime = _storyDateTimeOffset.ToUnixTimeSeconds();

    public static StoryDto CreateStory(long id, int score)
    {
        return new StoryDto(
            id,
            "test-user",
            10,
            score,
            _unixStoryTime,
            $"Story {id}",
            "story",
            $"https://example.com/{id}");
    }

    public static CompactStoryDto CreateCompactStory(long id, int score)
    {
        return new CompactStoryDto(
            $"Story {id}",
            $"https://example.com/{id}",
            "test-user",
            _storyDateTimeOffset,
            score,
            10);
    }
}