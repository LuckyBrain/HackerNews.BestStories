namespace HackerNews.BestStories.Api.Adapters;

using Models;

public sealed class StoryAdapter : IStoryAdapter
{
    public CompactStoryDto Adapt(StoryDto story)
    {
        return new CompactStoryDto(
            story.Title,
            story.Url,
            story.By,
            DateTimeOffset.FromUnixTimeSeconds(story.Time),
            story.Score,
            story.Descendants);
    }
}