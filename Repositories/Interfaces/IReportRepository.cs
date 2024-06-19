namespace FireEscape.Repositories.Interfaces;

public interface IReportRepository
{
    Task CreateReportAsync(Order order, Protocol protocol, string outputPath);
}
