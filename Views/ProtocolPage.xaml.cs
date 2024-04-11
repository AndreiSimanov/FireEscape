namespace FireEscape.Views;

public partial class ProtocolPage : ContentPage
{
    public ProtocolPage(ProtocolViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    void ContentPage_Disappearing(object sender, EventArgs e)
    {
        var viewModel = BindingContext as ProtocolViewModel;
        viewModel?.SaveProtocolCommand.Execute(null);
    }
}