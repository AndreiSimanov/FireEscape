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
    BatchReportStatusEnum batchReportStatusEnum = BatchReportStatusEnum.Start;

    [ObservableProperty]
    double progress;

    object syncObject = new();
    bool disposed;
    List<CancellationTokenSource> cancelTokenSources = new();

    [RelayCommand]
    void CreateReport()
    {
        DoCommand(() =>
        {
            if (Order == null || Protocols == null)
                return;

            if (Protocols.Length == 0)
                return; // add message "Protocols doesn't exist"

            if (BatchReportStatusEnum == BatchReportStatusEnum.Stop)
            {
                cancelTokenSources.LastOrDefault()?.Cancel();
                BatchReportStatusEnum = BatchReportStatusEnum.Start;
                return;
            }

            BatchReportStatusEnum = BatchReportStatusEnum.Stop;
            var progressIndicator = new Progress<(double progress, string outputPath)>(progress =>
            {
                Progress = progress.progress;
            });
            var cts = new CancellationTokenSource();
            cancelTokenSources.Add(cts);
            Task.Run(() => reportService.CreateBatchReportAsync(Order, Protocols, progressIndicator, cts.Token)).
                ContinueWith(_ => BatchReportStatusEnum = BatchReportStatusEnum.Start);
        },
        AppResources.CreateReportError);
    }

    public void Dispose()
    {
        if (disposed)
            return;

        lock (syncObject)
        {
            if (disposed)
                return;
            cancelTokenSources.ForEach(cts => cts.Cancel());
            cancelTokenSources.ForEach(cts => cts.Dispose());
            disposed = true;
        }
    }
}
