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




var api = builder.AddProject<Projects.FitnessApi>("fitnessapi")
    .WaitFor(ollama)
    .WithReference(ollama)
    .WithReference(mongodb);

api.WithUrl($"{api.GetEndpoint("http")}/scalar/v1", "OpenApi");



builder.Build().Run();
