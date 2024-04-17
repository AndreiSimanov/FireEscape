namespace FireEscape.Views;

public partial class OrderMainPage : ContentPage
{
    public OrderMainPage(OrderMainViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
    OrderMainViewModel? OrderMainViewModel => BindingContext as OrderMainViewModel;

    void CreateOrder(object sender, EventArgs e)
    {
        OrderMainViewModel?.AddOrderCommand.Execute(null);
        orders.ScrollTo(0);
    }
    void SearchTextChanged(object sender, EventArgs e)
    {
        OrderMainViewModel?.GetOrdersCommand.Execute(null);
    }

    private void ClearSearch(object sender, System.ComponentModel.HandledEventArgs e)
    {
        if (OrderMainViewModel == null)
            return;
        OrderMainViewModel.Search = string.Empty;
        OrderMainViewModel.GetOrdersCommand.Execute(null);
    }

    void CollectionViewChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (OrderMainViewModel != null)
            OrderMainViewModel.IsEmptyList = orders.VisibleItemCount == 0 && !OrderMainViewModel.IsRefreshing;
    }
}