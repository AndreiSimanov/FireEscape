namespace FireEscape.Views;

public partial class BatchReportPage : ContentPage
{
    public BatchReportPage(BatchReportViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    BatchReportViewModel? BatchReportViewModel => BindingContext as BatchReportViewModel;

    private void ContentPageAppearing(object sender, EventArgs e)
    {
        BatchReportViewModel?.GetReportsCommand.Execute(null);
    }

    private void ContentPageDisappearing(object sender, EventArgs e)
    {
        BatchReportViewModel?.ResetCommand.Execute(null);
    }
}