namespace HackerNews.BestStories.Api.Adapters;

using Models;

public interface IStoryAdapter
{
    CompactStoryDto Adapt(StoryDto story);
}