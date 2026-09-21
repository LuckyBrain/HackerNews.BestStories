namespace HackerNews.BestStories.TestHelpers;

using Api.Adapters;
using Api.Models;

public sealed class StubOfStoryAdapter : IStoryAdapter
{
    private static readonly DateTimeOffset StoryTime = new(2019, 10, 12, 13, 43, 1, TimeSpan.Zero);

    public CompactStoryDto Adapt(StoryDto story)
    {
        return new CompactStoryDto(
            $"Story {story.Id}",
            $"https://example.com/{story.Id}",
            "test-user",
            StoryTime,
            story.Score,
            10);
    }
}