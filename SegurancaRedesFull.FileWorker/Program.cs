using SegurancaRedesFull.FileWorker;
using SegurancaRedesFull.FileWorker.Options;
using SegurancaRedesFull.FileWorker.Services;
using System.Diagnostics.Metrics;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.Configure<FileWorkerOptions>(
    builder.Configuration.GetSection("FileWorkerOptions")
);

builder.Services.Configure<FileManagerOptions>(
    builder.Configuration.GetSection("FileManagerOptions")
);

builder.Services.AddSingleton(new Meter("FileWorkerCustom", "1.0")).AddMetrics();

builder.Services.AddSingleton<FileManager>();
builder.Services.AddSingleton<FileMetrics>();

builder.Services.AddHostedService<FileManagerWorker>();
builder.Services.AddHostedService<MetricUpdaterWorker>();

var host = builder.Build();
host.Run();
