namespace FireEscape.Views;

public partial class RemoteLogPage : ContentPage
{
    public RemoteLogPage(RemoteLogViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    RemoteLogViewModel? RemoteLogViewModel => BindingContext as RemoteLogViewModel;

    private void ContentPageAppearing(object sender, EventArgs e)
    {
        RemoteLogViewModel?.GetBatchReportLogCommand.Execute(null);
    }

    private void ContentPageDisappearing(object sender, EventArgs e)
    {
        RemoteLogViewModel?.ResetCommand.Execute(null);
    }
}