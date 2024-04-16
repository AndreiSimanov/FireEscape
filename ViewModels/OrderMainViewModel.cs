using CommunityToolkit.Maui.Core.Extensions;
using FireEscape.Resources.Languages;
using System.Collections.ObjectModel;

namespace FireEscape.ViewModels
{
    public partial class OrderMainViewModel(OrderService orderService, UserAccountService userAccountService) : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<Order> orders = new();
        [ObservableProperty]
        bool isRefreshing;

        [ObservableProperty]
        bool isEmptyList = true;

    
        [RelayCommand]
        async Task GetOrdersAsync() => //!! todo: add paging
            await DoCommandAsync(async () =>
            {
                try
                {
                    if (Orders.Any())
                        return;
                    IsRefreshing = true;

                    var orders = await orderService.GetOrdersAsync();
                    Orders = orders.ToObservableCollection();
                }
                finally
                {
                    IsRefreshing = false;
                }
            },
            AppResources.GetOrdersError);

        [RelayCommand]
        async Task AddOrderAsync()
        {
            Order? newOrder = null;
            await DoCommandAsync(async () =>
            {
                newOrder = await orderService.CreateOrderAsync();
                Orders.Insert(0, newOrder);
            },
            AppResources.AddOrderError);

            if (newOrder != null)
                await GoToDetailsAsync(newOrder);
        }


        [RelayCommand]
        async Task DeleteOrderAsync(Order order) =>
            await DoCommandAsync(async () =>
            {
                var action = await Shell.Current.DisplayActionSheet(AppResources.DeleteOrder, AppResources.Cancel, AppResources.Delete);

                if (string.Equals(action, AppResources.Cancel))
                    return;

                await orderService.DeleteOrderAsync(order);
                Orders.Remove(order);
            },
            order,
            AppResources.DeleteOrderError);



        [RelayCommand]
        async Task GoToDetailsAsync(Order order) =>
            await DoCommandAsync(async () =>
            {
                await Shell.Current.GoToAsync(nameof(OrderPage), true, new Dictionary<string, object> { { nameof(Order), order } });
            },
            order,
            AppResources.EditOrderError);


        [RelayCommand]
        async Task GoToProtocolsAsync(Order order) =>
            await DoCommandAsync(async () =>
            {
                await Shell.Current.GoToAsync(nameof(ProtocolMainPage), true, new Dictionary<string, object> { { nameof(Order), order } });
            },
            order,
            AppResources.EditOrderError);


        [RelayCommand]
        async Task OpenUserAccountMainPageAsync() =>
            await DoCommandAsync(async () =>
            {
                var userAccount = await userAccountService.GetCurrentUserAccount(true);
                if (UserAccountService.IsValidUserAccount(userAccount) && userAccount!.IsAdmin)
                {
                    await Shell.Current.GoToAsync(nameof(UserAccountMainPage), true);
                }
            },
            AppResources.OpenUserAccountMainPageError);

        [RelayCommand] //For test only 
        async Task AddOrdersAsync() => 
            await DoCommandAsync(async () =>
            {
                for (var i = 0; i < 1000; i++)
                {
                    await orderService.CreateOrderAsync();
                }
            },
            AppResources.AddOrderError);
    }
}
