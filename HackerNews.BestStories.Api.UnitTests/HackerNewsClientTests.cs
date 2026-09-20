namespace HackerNews.BestStories.Api.UnitTests;

using System.Net;
using System.Net.Http.Json;
using Clients;
using Exceptions;
using Helpers;
using Microsoft.Extensions.DependencyInjection;

public class HackerNewsClientTests
{
    public class GetBestStoryIdsTests
    {
        private const int MaxRetryAttempts = 3;

        [Fact]
        public async Task WhenApiCannotBeReached_ShouldThrow()
        {
            var expectedInnerException = new HttpRequestException("API cannot be reached");
            var handler = new TestHttpMessageHandlerStub((_, _) => Task.FromException<HttpResponseMessage>(expectedInnerException));
            using var httpClient = new HttpClient(handler);
            var sut = new HackerNewsClient(httpClient);

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
            var sut = serviceProvider.GetRequiredService<HackerNewsClient>();

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
                    if (actualRequestCount == ValidRetryNo)
                    {
                        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
                        return Task.FromResult(httpResponseMessage);
                    }

                    var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(new[] { 100L, 200L, 300L }) };
                    return Task.FromResult(responseMessage);
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
            var sut = serviceProvider.GetRequiredService<HackerNewsClient>();

            var actual = await sut.GetBestStoryIdsAsync();

            Assert.Equal(new[] { 100L, 200L, 300L }, actual);
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
                    var jsonContent = JsonContent.Create(new[] { 100L, 200L, 300L });
                    var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = jsonContent };
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
            var sut = serviceProvider.GetRequiredService<HackerNewsClient>();

            var actual = await sut.GetBestStoryIdsAsync();

            // Assert
            Assert.Equal(new[] { 100L, 200L, 300L }, actual);
            Assert.Equal(1, actualRequestCount);
        }
    }
}