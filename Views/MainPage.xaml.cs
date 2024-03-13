namespace FireEscape.View;

public partial class MainPage : ContentPage
{
    public MainPage(ProtocolsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        var viewModel = BindingContext as ProtocolsViewModel;
        viewModel?.GetProtocolsCommand.Execute(null);
    }
}
