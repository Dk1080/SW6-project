using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);


//Add database with express for monitoring.
var mongo = builder
    .AddMongoDB("mongo")
    .WithMongoExpress()
    .WithDataVolume();

var mongodb = mongo.AddDatabase("FitnessAppDatabase");

//Setup ollama support
var ollama = builder.AddOllama(name: "ollama")
    .WithGPUSupport(OllamaGpuVendor.Nvidia)
    //.WithOpenWebUI()
    .WithDataVolume()
    .AddModel("llama3.2");




builder.AddProject<Projects.FitnessApi>("fitnessapi")
    .WaitFor(ollama)
    .WithReference(ollama)
    .WithReference(mongodb);

builder.Build().Run();
