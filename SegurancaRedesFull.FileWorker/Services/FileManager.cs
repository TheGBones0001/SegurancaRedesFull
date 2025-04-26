using Microsoft.Extensions.Options;
using SegurancaRedesFull.FileWorker.Options;
using System.Diagnostics.Metrics;
using System.Text;

namespace SegurancaRedesFull.FileWorker.Services;

public class FileManager
{
    private readonly string _basePath;
    private readonly string _movedPath;
    private readonly ILogger<FileManager> _logger;

    private readonly Counter<int> _filesCreated;
    private readonly Counter<int> _filesMoved;
    private readonly Counter<int> _filesDeleted;

    public FileManager(ILogger<FileManager> logger, IOptions<FileManagerOptions> options, Meter meter)
    {
        _logger = logger;
        _basePath = options.Value.BasePath;
        _movedPath = options.Value.MovedPath;

        if (!Directory.Exists(_basePath))
            Directory.CreateDirectory(_basePath);

        if (!Directory.Exists(_movedPath))
            Directory.CreateDirectory(_movedPath);

        _filesCreated = meter.CreateCounter<int>("filemanager.files_created");
        _filesMoved = meter.CreateCounter<int>("filemanager.files_moved");
        _filesDeleted = meter.CreateCounter<int>("filemanager.files_deleted");
    }

    public Task CreateFile()
    {
        string filePath = Path.Combine(_basePath, $"file_{Guid.NewGuid()}.txt");

        int size = Random.Shared.Next(100, 1_000_000);
        string content = GenerateRandomText(size);
        File.WriteAllText(filePath, content);

        _filesCreated.Add(1);
        _logger.LogInformation("Arquivo criado: {filePath} (tamanho: {size} caracteres)", filePath, size);

        return Task.CompletedTask;
    }

    public Task MoveFile()
    {
        string[] files = Directory.GetFiles(_basePath);

        if (files.Length < 1)
            return Task.CompletedTask;

        string file = files[Random.Shared.Next(files.Length)];
        string newFile = Path.Combine(_movedPath, $"mvd_{Path.GetFileName(file)}");
        File.Move(file, newFile);

        _filesMoved.Add(1);
        _logger.LogInformation("Arquivo movido: {old} -> {new}", file, newFile);

        return Task.CompletedTask;
    }

    public Task DeleteFile()
    {
        string[] files = Directory.GetFiles(_movedPath);

        if (files.Length < 1)
            return Task.CompletedTask;

        string file = files[Random.Shared.Next(files.Length)];
        File.Delete(file);

        _filesDeleted.Add(1);
        _logger.LogInformation("Arquivo deletado: {file}", file);

        return Task.CompletedTask;
    }

    private string GenerateRandomText(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
        StringBuilder builder = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            builder.Append(chars[Random.Shared.Next(chars.Length)]);
        }

        return builder.ToString();
    }
}

