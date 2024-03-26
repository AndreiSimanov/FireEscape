namespace FireEscape.Views;

public partial class FireEscapePage : ContentPage
{
    public FireEscapePage(FireEscapeViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}