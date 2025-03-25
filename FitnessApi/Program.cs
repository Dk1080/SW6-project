using FitnessApi.Endpoints;
using FitnessApi.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Hosting;
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

//Configure session management.
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


//Adding ollama
builder.AddOllamaApiClient("chat").AddChatClient();
//builder.Services.AddChatClient(new OllamaChatClient(new Uri("http://localhost:59443/"), "gemma3:4b"));


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



