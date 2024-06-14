using FireEscape.Converters;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FireEscape.Services;

public class RemoteLogService(IFileHostingRepository fileHostingRepository, ILogger<RemoteLogService> logger, IOptions<RemoteLogSettings> remoteLogSettings)
{
    readonly RemoteLogSettings remoteLogSettings = remoteLogSettings.Value;

    public async Task LogAsync(string key, RemoteLogCategoryType remoteLogCategory, string message)
    {

        var log = GetLocalLog(remoteLogCategory);
        var categorySettings = remoteLogSettings.RemoteLogCategories?.FirstOrDefault(cat => cat.CategoryType == remoteLogCategory);
        var maxLogItemsCount = categorySettings.HasValue ? categorySettings.Value.MaxLogItemsCount : remoteLogSettings.DefaultMaxLogItemsCount;
        message = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss ") + message;

        var lastCount = log.Length < maxLogItemsCount ? log.Length : maxLogItemsCount - 1;
        log = [.. log.TakeLast(lastCount), .. new[] { message }];
        SetLocalLog(remoteLogCategory, log);
        await TryToUploadLogAsync(key, remoteLogCategory, log);
    }

    string[] GetLocalLog(RemoteLogCategoryType remoteLogCategory)
    {
        var json = Preferences.Default.Get(EnumDescriptionTypeConverter.GetEnumDescription(remoteLogCategory), string.Empty);
        return string.IsNullOrWhiteSpace(json) ? [] : JsonSerializer.Deserialize<string[]>(json) ?? [];
    }

    void SetLocalLog(RemoteLogCategoryType remoteLogCategory, string[] messages)
    {
        var json = JsonSerializer.Serialize(messages);
        Preferences.Set(EnumDescriptionTypeConverter.GetEnumDescription(remoteLogCategory), json);
    }

    async Task TryToUploadLogAsync(string key, RemoteLogCategoryType remoteLogCategory, string[] messages)
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

    public async Task<string[]> TryToDownloadLogAsync(string key, RemoteLogCategoryType remoteLogCategory)
    {
        if (string.IsNullOrWhiteSpace(key) || !await AppUtils.IsNetworkAccessAsync())
            return [];

        var folder = Path.Join(remoteLogSettings.RemoteLogFolderName, EnumDescriptionTypeConverter.GetEnumDescription(remoteLogCategory));
        var json = await fileHostingRepository.DownloadJsonAsync(key, folder);
        return string.IsNullOrWhiteSpace(json) ? [] : JsonSerializer.Deserialize<string[]>(json) ?? [];
    }
}
