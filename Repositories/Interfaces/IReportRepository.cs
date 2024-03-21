namespace FireEscape.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<string> CreateReportAsync(Protocol protocol, string filePath);
    }
}
