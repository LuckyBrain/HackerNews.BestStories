namespace HackerNews.BestStories.Api;

using Adapters;
using Clients;
using ErrorHandling;
using Microsoft.Extensions.Caching.Hybrid;
using Services;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<HackerNewsExceptionHandler>();

        builder.Services.AddHybridCache();

        builder.Services
            .AddHttpClient<HackerNewsClient>()
            .AddStandardResilienceHandler();

        builder.Services
            .AddScoped<IHackerNewsClient>(
                serviceProvider =>
                {
                    var innerClient = serviceProvider.GetRequiredService<HackerNewsClient>();
                    var cache = serviceProvider.GetRequiredService<HybridCache>();
                    return new CachingHackerNewsClient(innerClient, cache);
                })
            .AddScoped<IHackerNewsService, HackerNewsService>()
            .AddScoped<IStoryAdapter, StoryAdapter>()
            .AddScoped<ICompactStoriesService, CompactStoriesService>();

        var app = builder.Build();

        app.UseExceptionHandler();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}