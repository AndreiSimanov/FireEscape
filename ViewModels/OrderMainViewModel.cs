using CommunityToolkit.Maui.Core.Extensions;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;

namespace FireEscape.ViewModels;

public partial class OrderMainViewModel(IOptions<ApplicationSettings> applicationSettings, OrderService orderService, ProtocolService protocolService,
    UserAccountService userAccountService, ReportService reportService, ILogger<OrderMainViewModel> logger) : BaseViewModel(logger)
{
    readonly ApplicationSettings applicationSettings = applicationSettings.Value;
    PagingParameters pageParams;

    [ObservableProperty]
    ObservableCollection<Order> orders = new();

    [ObservableProperty]
    bool isRefreshing;

    [ObservableProperty]
    bool isLoadMore;

    [ObservableProperty]
    string search = string.Empty;

    [ObservableProperty]
    bool isEmptyList = true;

    [RelayCommand]
    async Task GetOrdersAsync() =>
        await DoCommandAsync(async () =>
        {
            try
            {
                IsRefreshing = true;
                IsLoadMore = true;
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
    async Task LoadMoreAsync() =>
        await DoCommandAsync(async () =>
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
    async Task AddOrderAsync()
    {
        Order? newOrder = null;
        await DoCommandAsync(async () =>
        {
            newOrder = await orderService.CreateAsync();
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

            await orderService.DeleteAsync(order);
            Orders.Remove(order);
        },
        order,
        AppResources.DeleteOrderError);

    [RelayCommand]
    async Task GoToDetailsAsync(Order order) =>
        await DoCommandAsync(async () =>
        {
            await Shell.Current.GoToAsync(nameof(OrderPage), true,
                new Dictionary<string, object> { { nameof(OrderViewModel.EditObject), order } });
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
    async Task CreateReportAsync(Order order) =>
        await DoCommandAsync(async () =>
        {
            if (order == null)
                return;
            var protocols = await protocolService.GetProtocolsAsync(order.Id);
            if (!protocols.Any())
                return;
             await reportService.CreateBatchReportAsync(order, protocols);
        },
        order,
        AppResources.CreateReportError);

    [RelayCommand]
    async Task OpenUserAccountMainPageAsync() =>
        await DoCommandAsync(async () =>
        {
            var userAccount = await userAccountService.GetCurrentUserAccountAsync(true);
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

    static Random random = new Random();
    static string RandomString(int length)
    {
        length = random.Next(0, length);
        const string chars = "ЙЦУКЕНГШЩЗХЪЭЖДЛОРПАВЫФЯЧСМИТЬБЮйцукенгшщзхъфывапролджэячсмитьбю., 0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
