namespace FireEscape.Views;

public partial class RemoteLogPage : ContentPage
{
    public RemoteLogPage(RemoteLogViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    RemoteLogViewModel? RemoteLogViewModel => BindingContext as RemoteLogViewModel;

    async void ContentPageAppearing(object sender, EventArgs e)
    {
        if (RemoteLogViewModel != null)
            await RemoteLogViewModel.GetBatchReportLogCommand.ExecuteAsync(null);
    }

    void ContentPageDisappearing(object sender, EventArgs e)
    {
        RemoteLogViewModel?.ResetCommand.Execute(null);
    }
}