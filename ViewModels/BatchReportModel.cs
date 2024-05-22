namespace FireEscape.ViewModels;

[QueryProperty(nameof(Order), nameof(Order))]
[QueryProperty(nameof(Protocols), nameof(Protocol))]
public partial class BatchReportModel(ReportService reportService, ILogger<BatchReportModel> logger) : BaseViewModel(logger)
{
    [ObservableProperty]
    Order? order;

    [ObservableProperty]
    Protocol[]? protocols;

    [ObservableProperty]
    string progressBarButtonContent = "Start";

    [ObservableProperty]
    double progress;

    [RelayCommand]
    async Task CreateReportAsync() =>
        await DoCommandAsync(async () =>
        {
            if (Order == null || Protocols == null)
                return;

            if (!Protocols.Any())
                return; // add message "Protocols doesn't exist"
            await reportService.CreateBatchReportAsync(Order, Protocols);
        },
        Order,
        AppResources.CreateReportError);
}
