using System.Collections.ObjectModel;

namespace FireEscape.ViewModels;

[QueryProperty(nameof(Order), nameof(Order))]
[QueryProperty(nameof(Protocols), nameof(Protocol))]
public partial class BatchReportModel(ReportService reportService, ILogger<BatchReportModel> logger) : BaseViewModel(logger) , IDisposable
{
    [ObservableProperty]
    Order? order;

    [ObservableProperty]
    Protocol[]? protocols;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HeaderVisible))]
    ObservableCollection<OutpuFile> outputFiles = new();

    [ObservableProperty]
    StartStopEnum startStopStatus = StartStopEnum.Start;

    [ObservableProperty]
    double progress;

    object syncObject = new();
    bool disposed;
    CancellationTokenSource? cts;

    [ObservableProperty]
    bool headerVisible;

    [RelayCommand]
    void CancelOperation() =>
        DoCommand(() =>
        {
            if (StartStopStatus == StartStopEnum.Stop)
            {
                cts?.Cancel();
                StartStopStatus = StartStopEnum.Start;
            }
        },
        AppResources.CreateReportError);

    [RelayCommand]
    async Task CreateReportAsync(Order order) =>
        await DoCommandAsync(async () =>
        {
            if (Order == null || Protocols == null)
                return;

            if (Protocols.Length == 0)
                return; // add message "Protocols doesn't exist"

            StartStopStatus = StartStopEnum.Stop;
            HeaderVisible = false;
            OutputFiles.Clear();

            var progressIndicator = new Progress<(double progress, string outputPath)>(progress =>
            {
                OutputFiles.Add(new OutpuFile() { FilePath = progress.outputPath });
                HeaderVisible = true;
                Progress = progress.progress;
            });

            try
            {
                cts = new CancellationTokenSource();
                await reportService.CreateBatchReportAsync(Order, Protocols, progressIndicator, cts.Token);
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

    [RelayCommand]
    async Task OpenFileAsync(OutpuFile outpuFile) =>
        await DoCommandAsync(async () =>
        {
            await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(outpuFile.FilePath) });
        },
        outpuFile,
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
}
