namespace FireEscape.Views;

public partial class OrderMainPage : ContentPage
{
    public OrderMainPage(OrderMainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    OrderMainViewModel? OrderMainViewModel => BindingContext as OrderMainViewModel;

    async void CreateOrder(object sender, EventArgs e)
    {
        if (OrderMainViewModel != null)
        {
            await OrderMainViewModel.AddOrderCommand.ExecuteAsync(null);
            orders.ScrollTo(0);
        }
    }

    void CollectionViewChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (OrderMainViewModel != null)
            OrderMainViewModel.IsEmptyList = orders.VisibleItemCount == 0 && !OrderMainViewModel.IsRefreshing;
    }

    async void ScrolledAsync(object sender, DevExpress.Maui.CollectionView.DXCollectionViewScrolledEventArgs e) => await searchControl.HideKeyboardAsync();
}