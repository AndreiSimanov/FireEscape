namespace FireEscape.Views;

public partial class OrderPage : BaseOrderPage<OrderViewModel>
{
	public OrderPage(OrderViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}