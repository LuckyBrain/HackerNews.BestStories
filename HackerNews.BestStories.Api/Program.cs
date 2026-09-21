namespace HackerNews.BestStories.Api;

using Adapters;
using Clients;
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

        builder.Services
            .AddHttpClient<IHackerNewsClient, HackerNewsClient>()
            .AddStandardResilienceHandler();

        builder.Services
            .AddScoped<IHackerNewsService, HackerNewsService>()
            .AddScoped<IStoryAdapter, StoryAdapter>()
            .AddScoped<ICompactStoriesService, CompactStoriesService>();

        var app = builder.Build();

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