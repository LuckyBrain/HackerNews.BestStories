namespace HackerNews.BestStories.Api.UnitTests;

using System.Net;
using System.Net.Http.Json;
using Clients;
using Exceptions;
using Helpers;
using Microsoft.Extensions.DependencyInjection;
using Models;

public class HackerNewsClientTests
{
    private const int MaxRetryAttempts = 3;

    public class GetBestStoryIdsTests
    {
        private static readonly long[] _expectedStoryIds = { 100L, 200L, 300L };

        private static HttpResponseMessage CreateOkMessage()
        {
            var jsonContent = JsonContent.Create(_expectedStoryIds);
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = jsonContent };
            return httpResponseMessage;
        }

        [Fact]
        public async Task WhenApiCannotBeReached_ShouldThrow()
        {
            var expectedInnerException = new HttpRequestException("API cannot be reached");
            var handler = new TestHttpMessageHandlerStub((_, _) => Task.FromException<HttpResponseMessage>(expectedInnerException));
            using var httpClient = new HttpClient(handler);
            IHackerNewsClient sut = new HackerNewsClient(httpClient);

            var actualException = await Assert.ThrowsAsync<ApiUnreachableException>(() => sut.GetBestStoryIdsAsync());

            var actualInnerException = actualException.InnerException;
            Assert.IsType(expectedInnerException.GetType(), actualInnerException);
            Assert.Equal(expectedInnerException.Message, actualInnerException.Message);
        }

        [Fact]
        public async Task WhenApiCanBeReached_WhenRequestFails_WhenAllRetriesFail_ShouldThrow()
        {
            var actualRequestCount = 0;
            var handler = new TestHttpMessageHandlerStub(
                (_, _) =>
                {
                    actualRequestCount++;
                    var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
                    return Task.FromResult(httpResponseMessage);
                });
            var services = new ServiceCollection();
            services
                .AddHttpClient<HackerNewsClient>()
                .ConfigurePrimaryHttpMessageHandler(() => handler)
                .AddStandardResilienceHandler(
                    options =>
                    {
                        options.Retry.MaxRetryAttempts = MaxRetryAttempts;
                        options.Retry.Delay = TimeSpan.Zero;
                        options.Retry.UseJitter = false;
                    });
            await using var serviceProvider = services.BuildServiceProvider();
            IHackerNewsClient sut = serviceProvider.GetRequiredService<HackerNewsClient>();

            await Assert.ThrowsAsync<ApiResponseException>(() => sut.GetBestStoryIdsAsync());

            Assert.Equal(1 + MaxRetryAttempts, actualRequestCount);
        }

        [Fact]
        public async Task WhenApiCanBeReached_WhenRequestFails_WhenRetrySucceeds_ShouldReturnIds()
        {
            const int ValidRetryNo = 1;
            var actualRequestCount = 0;
            var handler = new TestHttpMessageHandlerStub(
                (_, _) =>
                {
                    actualRequestCount++;
                    var httpResponseMessage = actualRequestCount == ValidRetryNo
                        ? new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                        : CreateOkMessage();
                    return Task.FromResult(httpResponseMessage);
                });
            var services = new ServiceCollection();
            services
                .AddHttpClient<HackerNewsClient>()
                .ConfigurePrimaryHttpMessageHandler(() => handler)
                .AddStandardResilienceHandler(
                    options =>
                    {
                        options.Retry.MaxRetryAttempts = MaxRetryAttempts;
                        options.Retry.Delay = TimeSpan.Zero;
                        options.Retry.UseJitter = false;
                    });
            await using var serviceProvider = services.BuildServiceProvider();
            IHackerNewsClient sut = serviceProvider.GetRequiredService<HackerNewsClient>();

            var actual = await sut.GetBestStoryIdsAsync();

            Assert.Equal(_expectedStoryIds, actual);
            Assert.Equal(1 + ValidRetryNo, actualRequestCount);
        }

        [Fact]
        public async Task WhenApiCanBeReached_WhenRequestSucceeds_ShouldReturnIds()
        {
            var actualRequestCount = 0;
            var handler = new TestHttpMessageHandlerStub(
                (_, _) =>
                {
                    actualRequestCount++;
                    var httpResponseMessage = CreateOkMessage();
                    return Task.FromResult(httpResponseMessage);
                });

            var services = new ServiceCollection();
            services
                .AddHttpClient<HackerNewsClient>()
                .ConfigurePrimaryHttpMessageHandler(() => handler)
                .AddStandardResilienceHandler(
                    options =>
                    {
                        options.Retry.MaxRetryAttempts = MaxRetryAttempts;
                        options.Retry.Delay = TimeSpan.Zero;
                        options.Retry.UseJitter = false;
                    });
            await using var serviceProvider = services.BuildServiceProvider();
            IHackerNewsClient sut = serviceProvider.GetRequiredService<HackerNewsClient>();

            var actual = await sut.GetBestStoryIdsAsync();

            // Assert
            Assert.Equal(_expectedStoryIds, actual);
            Assert.Equal(1, actualRequestCount);
        }
    }

    public class GetStoryTests
    {
        private const int MockStoryId = 234;

        private static readonly StoryDto _expectedStoryDto = new(
            MockStoryId,
            "test-user",
            25,
            100,
            1758390000,
            "Test story",
            "story",
            "https://example.com");

        private static HttpResponseMessage CreateOkMessage()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(_expectedStoryDto) };
            return httpResponseMessage;
        }

        [Fact]
        public async Task WhenApiCannotBeReached_ShouldThrow()
        {
            var expectedInnerException = new HttpRequestException("API cannot be reached");
            var handler = new TestHttpMessageHandlerStub((_, _) => Task.FromException<HttpResponseMessage>(expectedInnerException));
            using var httpClient = new HttpClient(handler);
            IHackerNewsClient sut = new HackerNewsClient(httpClient);

            var actualException = await Assert.ThrowsAsync<ApiUnreachableException>(() => sut.GetStoryAsync(MockStoryId));

            var actualInnerException = actualException.InnerException;
            Assert.IsType(expectedInnerException.GetType(), actualInnerException);
            Assert.Equal(expectedInnerException.Message, actualInnerException.Message);
        }

        [Fact]
        public async Task WhenApiCanBeReached_WhenRequestFails_WhenAllRetriesFail_ShouldThrow()
        {
            var actualRequestCount = 0;
            var handler = new TestHttpMessageHandlerStub(
                (_, _) =>
                {
                    actualRequestCount++;
                    var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
                    return Task.FromResult(httpResponseMessage);
                });
            var services = new ServiceCollection();
            services
                .AddHttpClient<HackerNewsClient>()
                .ConfigurePrimaryHttpMessageHandler(() => handler)
                .AddStandardResilienceHandler(
                    options =>
                    {
                        options.Retry.MaxRetryAttempts = MaxRetryAttempts;
                        options.Retry.Delay = TimeSpan.Zero;
                        options.Retry.UseJitter = false;
                    });
            await using var serviceProvider = services.BuildServiceProvider();
            IHackerNewsClient sut = serviceProvider.GetRequiredService<HackerNewsClient>();

            await Assert.ThrowsAsync<ApiResponseException>(() => sut.GetStoryAsync(MockStoryId));

            Assert.Equal(1 + MaxRetryAttempts, actualRequestCount);
        }

        [Fact]
        public async Task WhenApiCanBeReached_WhenRequestFails_WhenRetrySucceeds_WhenNotFound_ShouldThrow()
        {
            const int NotFoundAttempt = 2;
            var actualRequestCount = 0;
            var handler = new TestHttpMessageHandlerStub(
                (_, _) =>
                {
                    actualRequestCount++;
                    var httpResponseMessage = actualRequestCount == NotFoundAttempt
                        ? new HttpResponseMessage(HttpStatusCode.NotFound)
                        : new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
                    return Task.FromResult(httpResponseMessage);
                });

            var services = new ServiceCollection();
            services
                .AddHttpClient<HackerNewsClient>()
                .ConfigurePrimaryHttpMessageHandler(() => handler)
                .AddStandardResilienceHandler(
                    options =>
                    {
                        options.Retry.MaxRetryAttempts = MaxRetryAttempts;
                        options.Retry.Delay = TimeSpan.Zero;
                        options.Retry.UseJitter = false;
                    });
            await using var serviceProvider = services.BuildServiceProvider();
            IHackerNewsClient sut = serviceProvider.GetRequiredService<HackerNewsClient>();

            await Assert.ThrowsAsync<ApiResponseException>(() => sut.GetStoryAsync(MockStoryId));

            Assert.Equal(NotFoundAttempt, actualRequestCount);
        }

        [Fact]
        public async Task WhenApiCanBeReached_WhenRequestFails_WhenRetrySucceeds_WhenFound_ShouldReturnStory()
        {
            const int ValidRetryNo = 1;
            var actualRequestCount = 0;
            var handler = new TestHttpMessageHandlerStub(
                (_, _) =>
                {
                    actualRequestCount++;
                    var httpResponseMessage = actualRequestCount == ValidRetryNo
                        ? new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                        : CreateOkMessage();
                    return Task.FromResult(httpResponseMessage);
                });
            var services = new ServiceCollection();
            services
                .AddHttpClient<HackerNewsClient>()
                .ConfigurePrimaryHttpMessageHandler(() => handler)
                .AddStandardResilienceHandler(
                    options =>
                    {
                        options.Retry.MaxRetryAttempts = MaxRetryAttempts;
                        options.Retry.Delay = TimeSpan.Zero;
                        options.Retry.UseJitter = false;
                    });
            await using var serviceProvider = services.BuildServiceProvider();
            IHackerNewsClient sut = serviceProvider.GetRequiredService<HackerNewsClient>();

            var actual = await sut.GetStoryAsync(MockStoryId);

            Assert.Equal(_expectedStoryDto, actual);
            Assert.Equal(1 + ValidRetryNo, actualRequestCount);
        }

        [Fact]
        public async Task WhenApiCanBeReached_WhenRequestSucceeds_WhenFound_ShouldReturnStory()
        {
            var actualRequestCount = 0;
            var handler = new TestHttpMessageHandlerStub(
                (_, _) =>
                {
                    actualRequestCount++;
                    var httpResponseMessage = CreateOkMessage();
                    return Task.FromResult(httpResponseMessage);
                });

            var services = new ServiceCollection();
            services
                .AddHttpClient<HackerNewsClient>()
                .ConfigurePrimaryHttpMessageHandler(() => handler)
                .AddStandardResilienceHandler(
                    options =>
                    {
                        options.Retry.MaxRetryAttempts = MaxRetryAttempts;
                        options.Retry.Delay = TimeSpan.Zero;
                        options.Retry.UseJitter = false;
                    });
            await using var serviceProvider = services.BuildServiceProvider();
            IHackerNewsClient sut = serviceProvider.GetRequiredService<HackerNewsClient>();

            var actual = await sut.GetStoryAsync(MockStoryId);

            // Assert
            Assert.Equal(_expectedStoryDto, actual);
            Assert.Equal(1, actualRequestCount);
        }
    }
}