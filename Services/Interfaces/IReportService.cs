namespace FireEscape.Services.Interfaces;

public interface IReportService
{
    Task CreateBatchReportAsync(Order order, Protocol[] protocols, CancellationToken ct, IProgress<(double progress, string outputPath)>? progress = null);
    Task CreateSingleReportAsync(Order order, Protocol protocol, bool incrementFileNameIfExists = false);
    Task<IEnumerable<FileInfo>> GetReportsAsync(Order order);
}