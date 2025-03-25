using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);


//Add database with express for monitoring.
var mongo = builder
    .AddMongoDB("mongo")
    .WithMongoExpress()
    .WithDataVolume();

var mongodb = mongo.AddDatabase("FitnessAppDatabase");

//Setup ollama support
var ollama = builder.AddOllama("ollama")
    .WithGPUSupport(OllamaGpuVendor.Nvidia)
    .WithOpenWebUI()
    .WithDataVolume();
var chat = ollama.AddModel("chat", "gemma3:4b");




builder.AddProject<Projects.FitnessApi>("fitnessapi")
    .WaitFor(chat)
    .WithReference(chat)
    .WithReference(mongodb);

builder.Build().Run();
