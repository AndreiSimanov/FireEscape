using DevExpress.Maui.Editors;

namespace FireEscape.Views;

public partial class OrderMainPage : ContentPage
{
    public OrderMainPage(OrderMainViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
    OrderMainViewModel? OrderMainViewModel => BindingContext as OrderMainViewModel;

    void ContentPage_Appearing(object sender, EventArgs e)
    {
        orders.PullToRefreshCommand.Execute(null);
    }

    void CreateOrder(object sender, EventArgs e)
    {
        OrderMainViewModel?.AddOrderCommand.Execute(null);
        orders.ScrollTo(0);
    }
    void SearchTextChanged(object sender, EventArgs e)
    {
        var searchText = ((TextEdit)sender).Text;
        orders.FilterString = $"Contains([FullAddress], '{searchText}') " +
            $"or Contains([Name], '{searchText}') " +
            $"or Contains([Customer], '{searchText}') " +
            $"or Contains([ExecutiveCompany], '{searchText}')";
    }

    void CollectionViewChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (OrderMainViewModel != null)
            OrderMainViewModel.IsEmptyList = orders.VisibleItemCount == 0 && !OrderMainViewModel.IsRefreshing;
    }
}