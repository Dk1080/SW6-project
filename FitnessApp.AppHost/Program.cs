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
    .AddModel("phi4-mini");

var api = builder.AddProject<Projects.FitnessApi>("fitnessapi")
    .WaitFor(ollama)
    .WithReference(ollama)
    .WithReference(mongodb);

//Add hyperlink to scalar website
api.WithUrl($"{api.GetEndpoint("http")}/scalar/v1", "OpenApi");


//Add smart display app.
builder.AddNpmApp("FitnessWebApp", "../FitnessWebApp")
    .WaitFor(api)
   .WithReference(api)
    .WithEnvironment("BROWSER", "none")
    .WithHttpEndpoint(env: "VITE_PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();



builder.Build().Run();
