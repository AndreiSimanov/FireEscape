namespace FireEscape.Services.Interfaces;

public interface IRemoteLogService
{
    Task<RemoteLogMessage[]> DownloadLogAsync(string key, RemoteLogCategoryType remoteLogCategory);
    Task LogAsync(string key, RemoteLogCategoryType remoteLogCategory, string message);
}