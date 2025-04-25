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

//Add Api
var api = builder.AddProject<Projects.FitnessApi>("fitnessapi")
    .WaitFor(ollama)
    .WithReference(ollama)
    .WithReference(mongodb);

//Add smart display app.
builder.AddNpmApp("FitnessWebApp", "../FitnessWebApp")
   .WithReference(api)
    .WithEnvironment("BROWSER", "none")
    .WithHttpEndpoint(env: "VITE_PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();


builder.Build().Run();
