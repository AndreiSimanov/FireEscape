namespace FireEscape.Views;

public partial class OrderPage : ContentPage
{
	public OrderPage(OrderViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    void ContentPage_Disappearing(object sender, EventArgs e)
    {
        var viewModel = BindingContext as OrderViewModel;
        viewModel?.SaveOrderCommand.Execute(null);
    }
}