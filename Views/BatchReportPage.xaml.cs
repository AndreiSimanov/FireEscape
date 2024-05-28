namespace FireEscape.Views;

public partial class BatchReportPage : ContentPage
{
    public BatchReportPage(BatchReportModel viewModel) 
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    BatchReportModel? BatchReportModel => BindingContext as BatchReportModel;

    private void ContentPageAppearing(object sender, EventArgs e)
    {
        BatchReportModel?.GetReportsCommand.Execute(null);
    }

    private void ContentPageDisappearing(object sender, EventArgs e)
    {
        var viewModel = BindingContext as  BatchReportModel;
        viewModel?.Dispose();
    }
}