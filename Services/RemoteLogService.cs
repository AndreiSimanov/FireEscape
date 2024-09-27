using FireEscape.Converters;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FireEscape.Services;

public class RemoteLogService(IFileHostingRepository fileHostingRepository, ILogger<RemoteLogService> logger, IOptions<RemoteLogSettings> remoteLogSettings) : IRemoteLogService
{
    readonly RemoteLogSettings remoteLogSettings = remoteLogSettings.Value;

    public Task LogAsync(string key, RemoteLogCategoryType remoteLogCategory, string message)
    {
        var log = GetLocalLog(remoteLogCategory);
        var categorySettings = remoteLogSettings.RemoteLogCategories?.FirstOrDefault(cat => cat.CategoryType == remoteLogCategory);
        var maxLogItemsCount = categorySettings.HasValue ? categorySettings.Value.MaxLogItemsCount : remoteLogSettings.DefaultMaxLogItemsCount;
        var remoteLogMessage = new RemoteLogMessage() { LogDateTime = DateTime.Now, СategoryType = remoteLogCategory, Message = message };

        var lastCount = log.Length < maxLogItemsCount ? log.Length : maxLogItemsCount - 1;
        log = [.. log.TakeLast(lastCount), .. new[] { remoteLogMessage }];
        SetLocalLog(remoteLogCategory, log);
        return UploadLogAsync(key, remoteLogCategory, log);
    }

    RemoteLogMessage[] GetLocalLog(RemoteLogCategoryType remoteLogCategory)
    {
        var json = Preferences.Default.Get(EnumDescriptionTypeConverter.GetEnumDescription(remoteLogCategory), string.Empty);
        return AppUtils.TryDeserialize<RemoteLogMessage[]>(json, out var result) ? result! : [];
    }

    void SetLocalLog(RemoteLogCategoryType remoteLogCategory, RemoteLogMessage[] messages)
    {
        var json = JsonSerializer.Serialize(messages);
        Preferences.Set(EnumDescriptionTypeConverter.GetEnumDescription(remoteLogCategory), json);
    }

    async Task UploadLogAsync(string key, RemoteLogCategoryType remoteLogCategory, RemoteLogMessage[] messages)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return;
        try
        {
            var folder = Path.Join(remoteLogSettings.RemoteLogFolderName, EnumDescriptionTypeConverter.GetEnumDescription(remoteLogCategory));
            await fileHostingRepository.UploadJsonAsync(key, JsonSerializer.Serialize(messages), folder);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
    }

    public async Task<RemoteLogMessage[]> DownloadLogAsync(string key, RemoteLogCategoryType remoteLogCategory)
    {
        if (string.IsNullOrWhiteSpace(key) || !await AppUtils.IsNetworkAccessAsync())
            return [];
        var folder = Path.Join(remoteLogSettings.RemoteLogFolderName, EnumDescriptionTypeConverter.GetEnumDescription(remoteLogCategory));
        var json = await fileHostingRepository.DownloadJsonAsync(key, folder);
        return AppUtils.TryDeserialize<RemoteLogMessage[]>(json, out var result) ? result! : [];
    }
}
