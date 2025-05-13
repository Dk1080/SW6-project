using FitnessApi.Endpoints;
using FitnessApi.Services;
using Microsoft.Extensions.AI;
using OllamaSharp;
using MongoDB.Driver;
using Polly;
using Polly.Retry;
using Scalar.AspNetCore;

namespace FitnessApi;
public class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        //Add conection to the database
        builder.AddMongoDBClient(connectionName: "FitnessAppDatabase");

        builder.Services.AddScoped<DatabaseContext>(svc =>
        {
            var scope = svc.CreateScope();
            return DatabaseContext.Create(scope.ServiceProvider.GetRequiredService<IMongoDatabase>());
        });


//Add DB services via DI
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChatHistoryService, ChatHistoryService>();
builder.Services.AddScoped<IChartDataService, ChartDataService>();
builder.Services.AddScoped<IHealthDataService, HealthDataService>();
builder.Services.AddScoped<IUserPreferencesService, UserPreferencesService>();



        //Configure session management.
        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });


        //Add Password hashing service
        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();


        //Adding ollama
        builder.AddOllamaApiClient("ollama-phi4-mini").AddChatClient(); //Also adds httpclient named "ollama-phi4-mini"

        //Reconfigure httpclient "ollama-phi4-mini"
        builder.Services.AddHttpClient("ollama-phi4-mini")
            .ConfigureHttpClient(client =>
            {
                client.Timeout = Timeout.InfiniteTimeSpan; //Remove built-in timeout
            })
            .AddResilienceHandler("Standard-TotalRequestTimeout", builder =>
            {
                builder.AddTimeout(TimeSpan.FromSeconds(60)); //Override Polly timeout  
            });


        var app = builder.Build();



        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapScalarApiReference(_ => _.Servers = []);
            app.MapOpenApi();
        }

        app.UseSession();

        app.UseHttpsRedirection();

        //Map endpoints
        app.MapLoginEndpoints();
        app.MapChatEndpoints();
        app.MapHealthDataEndpoints();
        app.MapDashboardEndpoints();

        app.Run();

    }


}





