using CommunityToolkit.Maui.Core.Extensions;
using System.Collections.ObjectModel;

namespace FireEscape.ViewModels;

[QueryProperty(nameof(Key), nameof(Key))]
public partial class RemoteLogViewModel(IRemoteLogService remoteLogService, ILogger<RemoteLogViewModel> logger) : BaseViewModel(logger)
{
    [ObservableProperty]
    string? key;

    [ObservableProperty]
    ObservableCollection<RemoteLogMessage> log = [];

    [ObservableProperty]
    bool isRefreshing;

    [RelayCommand]
    Task GetBatchReportLogAsync() =>
        DoBusyCommandAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(Key))
                return;

            IsRefreshing = true;
            var logResult = await remoteLogService.DownloadLogAsync(Key, RemoteLogCategoryType.BatchReport);
            Log = logResult.ToObservableCollection();
            IsRefreshing = false;
        },
        AppResources.GetRemoteLogError);

    [RelayCommand]
    void Reset() =>
        DoCommand(() =>
        {
            Log = [];
        },
        AppResources.GetRemoteLogError);
}
