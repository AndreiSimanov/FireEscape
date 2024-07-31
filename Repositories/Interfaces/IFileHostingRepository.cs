namespace FireEscape.Repositories.Interfaces;

public interface IFileHostingRepository
{
    Task DeleteJsonAsync(string key, string folder = "");
    Task DownloadAsync(string sourceFilePath, string destinationFilePath);
    IAsyncEnumerable<string> DownloadJsonAsync(IAsyncEnumerable<string> keys, string folder = "");
    Task<string> DownloadJsonAsync(string key, string folder = "");
    IAsyncEnumerable<string> ListFolderAsync(string folder);
    Task<string> UploadAsync(string sourceFilePath, string destinationFilePath);
    Task<string> UploadJsonAsync(string key, string value, string folder = "");
}