using SegurancaRedesFull.FileWorker.Services;

namespace SegurancaRedesFull.FileWorker;

public class MetricUpdaterWorker : BackgroundService
{
    private readonly FileMetrics _fileMetricsService;
    private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(10);

    public MetricUpdaterWorker(FileMetrics fileMetricsService)
    {
        _fileMetricsService = fileMetricsService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _fileMetricsService.ConfigureMetrics(); // Atualiza as métricas manualmente
            await Task.Delay(_updateInterval, stoppingToken);
        }
    }
}
