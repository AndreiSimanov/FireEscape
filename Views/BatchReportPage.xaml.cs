namespace FireEscape.Views;

public partial class BatchReportPage : ContentPage
{
    public BatchReportPage(BatchReportModel viewModel) 
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void ContentPageDisappearing(object sender, EventArgs e)
    {
        var viewModel = BindingContext as  BatchReportModel;
        viewModel?.Dispose();
    }
}