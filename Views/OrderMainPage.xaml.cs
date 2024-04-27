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

    void CollectionViewChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (OrderMainViewModel != null)
            OrderMainViewModel.IsEmptyList = orders.VisibleItemCount == 0 && !OrderMainViewModel.IsRefreshing;
    }
}