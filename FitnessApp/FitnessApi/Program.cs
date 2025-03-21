using FitnessApi.Endpoints;
using FitnessApi.Models;
using FitnessApi.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var mongoDBSettings = builder.Configuration.GetSection("MongoDBSettings").Get<MongoDBSettings>();
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));

builder.Services.AddDbContext<DatabaseContext>(options =>
options.UseMongoDB(mongoDBSettings.URI ?? "", mongoDBSettings.DatabaseName ?? ""));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}




app.UseHttpsRedirection();

app.MapLoginEndpoints();

app.MapGet("/GetData", () =>
{
    
});

app.Run();





