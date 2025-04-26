var builder = DistributedApplication.CreateBuilder(args);

var fileWorker = builder.AddProject<Projects.SegurancaRedesFull_FileWorker>("fileworker");

var apiService = builder.AddProject<Projects.SegurancaRedesFull_ApiService>("apiservice")
    .WaitFor(fileWorker);

builder.AddProject<Projects.SegurancaRedesFull_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
