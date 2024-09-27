using CommunityToolkit.Maui.Core.Extensions;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;

namespace FireEscape.ViewModels;

public partial class OrderMainViewModel(IOrderService orderService, IProtocolService protocolService, IUserAccountService userAccountService, 
    IReportService reportService, IOptions<ApplicationSettings> applicationSettings, ILogger<OrderMainViewModel> logger) : BaseViewModel(logger)
{
    readonly ApplicationSettings applicationSettings = applicationSettings.Value;
    PagingParameters pageParams;

    [ObservableProperty]
    ObservableCollection<Order> orders = [];

    [ObservableProperty]
    object? selectedItem = null;

    [ObservableProperty]
    bool isRefreshing;

    [ObservableProperty]
    bool isLoadMore;

    [ObservableProperty]
    string search = string.Empty;

    [ObservableProperty]
    bool isEmptyList = true;

    [RelayCommand]
    Task GetOrdersAsync() =>
        DoBusyCommandAsync(async () =>
        {
            try
            {
                IsRefreshing = true;
                IsLoadMore = true;
                SelectedItem = null;
                pageParams = new PagingParameters(0, applicationSettings.PageSize);
                var pagedResult = await orderService.GetOrdersAsync(Search, pageParams);
                IsLoadMore = pagedResult.IsLoadMore;
                Orders = pagedResult.Result.ToObservableCollection();
            }
            finally
            {
                IsRefreshing = false;
            }
        },
        AppResources.GetOrdersError);

    [RelayCommand]
    Task LoadMoreAsync() =>
        DoBusyCommandAsync(async () =>
        {
            if (!IsLoadMore)
                return;
            try
            {
                IsRefreshing = true;
                pageParams = new PagingParameters(pageParams.Skip + pageParams.Take, applicationSettings.PageSize);
                var pagedResult = await orderService.GetOrdersAsync(Search, pageParams);
                IsLoadMore = pagedResult.IsLoadMore;
                foreach (var order in pagedResult.Result)
                    Orders.Add(order);
            }
            finally
            {
                IsRefreshing = false;
            }
        },
        AppResources.GetOrdersError);

    [RelayCommand]
    Task AddOrderAsync() =>
        DoBusyCommandAsync(async () =>
        {
            var newOrder = await orderService.CreateAsync();
            Orders.Insert(0, newOrder);
            SelectedItem = newOrder;
            if (newOrder != null)
                await GoToAsync(newOrder);
        },
        AppResources.AddOrderError);

    [RelayCommand]
    Task DeleteOrderAsync(Order order) =>
        DoBusyCommandAsync(async () =>
        {
            SelectedItem = order;
            var action = await Shell.Current.DisplayActionSheet(AppResources.DeleteOrder, AppResources.Cancel, AppResources.Delete);

            if (string.Equals(action, AppResources.Delete))
            {
                await orderService.DeleteAsync(order);
                Orders.Remove(order);
                SelectedItem = null;
            }
        },
        order,
        AppResources.DeleteOrderError);

    [RelayCommand]
    Task GoToDetailsAsync(Order order) =>
        DoBusyCommandAsync(() => GoToAsync(order),
        order,
        AppResources.EditOrderError);


    [RelayCommand]
    Task GoToProtocolsAsync(Order order) =>
        DoBusyCommandAsync(() =>
        {
            SelectedItem = order;
            return Shell.Current.GoToAsync(nameof(ProtocolMainPage), true, new Dictionary<string, object> { { nameof(Order), order } });
        },
        order,
        AppResources.EditOrderError);

    [RelayCommand]
    Task CreateReportAsync(Order order) =>
        DoBusyCommandAsync(async () =>
        {
            SelectedItem = order;
            if (order == null)
                return;
            var protocols = await protocolService.GetProtocolsAsync(order.Id);

            if (protocols == null || protocols.Length == 0)
            {
                await Shell.Current.DisplayAlert(AppResources.Error, AppResources.OrderIsEmpty, AppResources.OK);
                return;
            }

            if (protocols.Length == 1)
            {
                await reportService.CreateSingleReportAsync(order, protocols[0]);
                return;
            }
            protocols = [.. protocols.OrderBy(protocol => protocol.FireEscapeNum)];
            await Shell.Current.GoToAsync(nameof(BatchReportPage), true, new Dictionary<string, object> { { nameof(Order), order }, { nameof(Protocol), protocols } });
        },
        order,
        AppResources.CreateReportError);

    [RelayCommand]
    Task OpenUserAccountMainPageAsync() =>
        DoBusyCommandAsync(async () =>
        {
            var userAccount = await userAccountService.GetCurrentUserAccountAsync(true);
            if (UserAccountService.IsValidUserAccount(userAccount) && userAccount!.IsAdmin)
            {
                await Shell.Current.GoToAsync(nameof(UserAccountMainPage), true);
            }
        },
        AppResources.OpenUserAccountMainPageError);

    Task GoToAsync(Order order)
    {
        SelectedItem = order;
        return Shell.Current.GoToAsync(nameof(OrderPage), true,
            new Dictionary<string, object> { { nameof(OrderViewModel.EditObject), order } });
    }

    [RelayCommand] //For test only 
    Task AddOrdersAsync() =>
        DoBusyCommandAsync(async () =>
        {
            for (var i = 0; i < 10000; i++)
            {
                var order = new Order
                {
                    Name = RandomString(15),
                    Location = RandomString(25),
                    Address = RandomString(35),
                    Customer = RandomString(15),
                    ExecutiveCompany = RandomString(15),
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                };
                await orderService.SaveAsync(order);
            }
        },
        AppResources.AddOrderError);

    static readonly Random random = new();
    static string RandomString(int length)
    {
        length = random.Next(0, length);
        const string chars = "ЙЦУКЕНГШЩЗХЪЭЖДЛОРПАВЫФЯЧСМИТЬБЮйцукенгшщзхъфывапролджэячсмитьбю., 0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
