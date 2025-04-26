using Microsoft.Extensions.Options;
using SegurancaRedesFull.FileWorker.Options;
using SegurancaRedesFull.FileWorker.Services;

namespace SegurancaRedesFull.FileWorker;

public class FileManagerWorker : BackgroundService
{
    private readonly ILogger<FileManagerWorker> _logger;
    private readonly FileManager _fileManager;
    private readonly int _delay;

    public FileManagerWorker(ILogger<FileManagerWorker> logger, IOptions<FileWorkerOptions> options, FileManager fileManager)
    {
        _logger = logger;
        _fileManager = fileManager;
        _delay = options.Value.DelayMilliseconds;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("FileWorker iniciado com delay de {_delay}ms.", _delay);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                int action = Random.Shared.Next(3);
                switch (action)
                {
                    case 0: await _fileManager.CreateFile(); break;
                    case 1: await _fileManager.MoveFile(); break;
                    case 2: await _fileManager.DeleteFile(); break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar operação de arquivo.");
            }

            await Task.Delay(_delay, stoppingToken);
        }

        _logger.LogInformation("FileWorker finalizado.");
    }
}
