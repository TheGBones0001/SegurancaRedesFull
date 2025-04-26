using Microsoft.Extensions.Options;
using SegurancaRedesFull.FileWorker.Options;
using System.Diagnostics.Metrics;

namespace SegurancaRedesFull.FileWorker.Services;
public class FileMetrics
{
    private readonly Meter _meter;
    private readonly string _basePath;
    private readonly string _movedPath;

    public FileMetrics(Meter meter, IOptions<FileManagerOptions> options)
    {
        _meter = meter;
        _basePath = options.Value.BasePath;
        _movedPath = options.Value.MovedPath;
    }

    public void ConfigureMetrics()
    {
        _meter.CreateObservableGauge<long>("disk.free_space.bytes", () => GetFreeDiskSpace("/"), "bytes");

        _meter.CreateObservableGauge<long>("base.files.total_size.bytes", () => GetTotalSize(_basePath), "bytes");
        _meter.CreateObservableGauge<long>("moved.files.total_size.bytes", () => GetTotalSize(_movedPath), "bytes");

        _meter.CreateObservableGauge<long>("base.files.avg_size.bytes", () => GetAverageFileSize(_basePath), "bytes");
        _meter.CreateObservableGauge<long>("moved.files.avg_size.bytes", () => GetAverageFileSize(_movedPath), "bytes");

        _meter.CreateObservableGauge<int>("base.files.count", () => Directory.GetFiles(_basePath).Length);
        _meter.CreateObservableGauge<int>("moved.files.count", () => Directory.GetFiles(_movedPath).Length);
    }

    public long GetFreeDiskSpace(string path)
    {
        var drive = DriveInfo.GetDrives().FirstOrDefault(d => path.StartsWith(d.Name));
        return drive?.AvailableFreeSpace ?? 0;
    }

    public long GetTotalSize(string path)
    {
        if (!Directory.Exists(path))
            return 0;

        try
        {
            var sizes = Directory.EnumerateFiles(path).Select(f => new FileInfo(f).Length).ToList();
            return sizes.Count > 0 ? sizes.Sum() : 0;
        }
        catch
        {
            return GetTotalSize(path);
        }
    }

    public long GetAverageFileSize(string path)
    {
        if (!Directory.Exists(path))
            return 0;

        try
        {
            var sizes = Directory.EnumerateFiles(path).Select(f => new FileInfo(f).Length).ToList();
            return sizes.Count > 0 ? (long)sizes.Average() : 0;
        }
        catch
        {
            return GetAverageFileSize(path);
        }
    }

}
