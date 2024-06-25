namespace FireEscape.Views;

public partial class BatchReportPage : ContentPage
{
    public BatchReportPage(BatchReportViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    BatchReportViewModel? BatchReportViewModel => BindingContext as BatchReportViewModel;

    void ContentPageAppearing(object sender, EventArgs e)
    {
        BatchReportViewModel?.GetReportsCommand.Execute(null);
    }

    void ContentPageDisappearing(object sender, EventArgs e)
    {
        BatchReportViewModel?.ResetCommand.Execute(null);
    }
}