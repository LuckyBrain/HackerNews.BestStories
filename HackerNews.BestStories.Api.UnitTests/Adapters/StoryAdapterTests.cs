using HackerNews.BestStories.Api.Adapters;

namespace HackerNews.BestStories.Api.UnitTests.Adapters;

using TestHelpers;

public class StoryAdapterTests
{
    [Fact]
    public void Adapt_ShouldReturnCompactStory()
    {
        var source = DtoFactory.CreateStory(123, 500);
        var expected = DtoFactory.CreateCompactStory(123, 500);
        IStoryAdapter sut = new StoryAdapter();

        var actual = sut.Adapt(source);

        Assert.Equal(expected, actual);
    }
}