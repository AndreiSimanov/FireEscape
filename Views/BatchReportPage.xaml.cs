namespace FireEscape.Views;

public partial class BatchReportPage : ContentPage
{
    public BatchReportPage(BatchReportModel viewModel) 
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}