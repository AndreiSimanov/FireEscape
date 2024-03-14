namespace FireEscape.Views;

public partial class ProtocolPage : ContentPage
{
    public ProtocolPage(ProtocolViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void ContentPage_Disappearing(object sender, EventArgs e)
    {
        var viewModel = BindingContext as ProtocolViewModel;
        if (viewModel != null)
        {
            viewModel.SaveProtocolCommand.Execute(null);
        }
    }
}