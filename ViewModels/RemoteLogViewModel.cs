using CommunityToolkit.Maui.Core.Extensions;
using System;
using System.Collections.ObjectModel;

namespace FireEscape.ViewModels;

[QueryProperty(nameof(Key), nameof(Key))]
public partial class RemoteLogViewModel(RemoteLogService remoteLogService, ILogger<RemoteLogViewModel> logger) : BaseViewModel(logger)
{
    [ObservableProperty]
    string? key;

    [ObservableProperty]
    ObservableCollection<RemoteLogMessage> log = [];

    [ObservableProperty]
    bool isRefreshing;

    [RelayCommand]
    async Task GetBatchReportLogAsync() =>
    await DoBusyCommandAsync(async () =>
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
