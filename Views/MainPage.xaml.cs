namespace FireEscape.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        var viewModel = BindingContext as MainViewModel;
        if (viewModel != null)
        {
            viewModel?.GetProtocolsCommand.Execute(null);
            if (viewModel?.SelectedProtocol != null)
            {
                var index = viewModel.Protocols.IndexOf(viewModel.SelectedProtocol);
                protocols.ScrollTo(index);
                viewModel.SelectedProtocol = null;
            }
        }
    }
}
