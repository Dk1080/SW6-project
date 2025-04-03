using FitnessApi.Endpoints;
using FitnessApi.Services;
using Microsoft.Extensions.AI;
using OllamaSharp;
using MongoDB.Driver;
using Scalar.AspNetCore;


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

//Configure session management.
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


//Adding ollama
builder.AddOllamaApiClient("ollama-llama3-2").AddChatClient();


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



app.Run();



