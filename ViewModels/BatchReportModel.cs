using CommunityToolkit.Maui.Core.Extensions;
using System.Collections.ObjectModel;

namespace FireEscape.ViewModels;

[QueryProperty(nameof(Order), nameof(Order))]
[QueryProperty(nameof(Protocols), nameof(Protocol))]
public partial class BatchReportModel(ReportService reportService, ILogger<BatchReportModel> logger) : BaseViewModel(logger), IDisposable
{
    [ObservableProperty]
    Order? order;

    [ObservableProperty]
    Protocol[]? protocols;

    [ObservableProperty]
    object? selectedItem = null;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilesExists))]
    ObservableCollection<FileInfo> files = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(MakeReportArchiveCommand))]
    [NotifyCanExecuteChangedFor(nameof(CreateReportCommand))]
    StartStopEnum startStopStatus = StartStopEnum.Start;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(MakeReportArchiveCommand))]
    bool filesExists;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateReportCommand))]
    bool isMakingReportArchive;

    [ObservableProperty]
    double progress;

    [ObservableProperty]
    double archiveProgress;

    object syncObject = new();
    bool disposed;
    CancellationTokenSource? cts;

    [RelayCommand]
    void CancelOperation() =>
        DoCommand(() =>
        {
            cts?.Cancel();
            StartStopStatus = StartStopEnum.Start;
        },
        AppResources.CreateReportError);

    [RelayCommand(CanExecute = nameof(CanCreateReport))]
    async Task CreateReportAsync(Order order) =>
        await DoBusyCommandAsync(async () =>
        {
            if (Protocols == null || Protocols.Length == 0)
            {
                await Shell.Current.DisplayAlert(AppResources.Error, AppResources.OrderIsEmpty, AppResources.OK);
                return;
            }

            StartStopStatus = StartStopEnum.Stop;
            FilesExists = false;
            SelectedItem = null;
            Files.Clear();

            var progressIndicator = new Progress<(double progress, string outputPath)>(progress =>
            {
                Files.Add(new FileInfo(progress.outputPath));
                FilesExists = true;
                Progress = progress.progress;
            });

            try
            {
                cts = new CancellationTokenSource();
                await reportService.CreateBatchReportAsync(Order!, Protocols, cts.Token, progressIndicator);
            }
            finally
            {
                cts?.Dispose();
                cts = null;
                StartStopStatus = StartStopEnum.Start;
            }
        },
        Order,
        AppResources.CreateReportError);

    [RelayCommand(CanExecute = nameof(CanMakeReportArchive))]
    async Task MakeReportArchiveAsync() =>
        await DoBusyCommandAsync(async () =>
        {
            if (!Files.Any())
                return;
            try
            {
                IsMakingReportArchive = true;
                var progressIndicator = new Progress<double>(progress => ArchiveProgress = progress);
                cts = new CancellationTokenSource();
                await reportService.MakeReportArchiveAsync(Files, cts.Token, progressIndicator);
            }
            finally
            {
                cts?.Dispose();
                cts = null;
                IsMakingReportArchive = false;
            }
        },
        Files,
        AppResources.CreateReportError);

    [RelayCommand]
    void GetReports() =>
        DoBusyCommand(() =>
        {
            if (Order == null)
                return;
            Files.Clear();
            FilesExists = false;
            SelectedItem = null;
            Files = reportService.GetReports(Order).ToObservableCollection();
            FilesExists = Files.Any();
        },
        AppResources.CreateReportError);

    [RelayCommand]
    async Task OpenFileAsync(FileInfo fileInfo) =>
        await DoCommandAsync(async () =>
        {
            SelectedItem = fileInfo;
            await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(fileInfo.FullName) });
        },
        fileInfo,
        AppResources.CreateReportError);

    public void Dispose()
    {
        if (disposed)
            return;

        lock (syncObject)
        {
            if (disposed)
                return;
            cts?.Cancel();
            cts?.Dispose();
            disposed = true;
        }
    }

    bool CanMakeReportArchive() => FilesExists && StartStopStatus == StartStopEnum.Start;
    bool CanCreateReport() => !IsMakingReportArchive;
}
