namespace FireEscape.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<string> CreateReportAsync(Protocol protocol,  UserAccount userAccount, string filePath);
    }
}
