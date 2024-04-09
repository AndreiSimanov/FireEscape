namespace FireEscape.Views;

public partial class StairsPage : ContentPage
{
    public StairsPage(StairsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}