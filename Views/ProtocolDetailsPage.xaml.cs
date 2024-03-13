namespace FireEscape.Views;

public partial class ProtocolDetailsPage : ContentPage
{
	public ProtocolDetailsPage(ProtocolsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}