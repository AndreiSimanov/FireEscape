using FireEscape.Resources.Languages;

namespace FireEscape.ViewModels
{
    [QueryProperty(nameof(Order), nameof(Order))]
    public partial class OrderViewModel(OrderService orderService) : BaseViewModel
    {
        [ObservableProperty]
        Order? order;

        [RelayCommand]
        async Task SaveOrderAsync() =>
            await DoCommandAsync(async () =>
            {
                await orderService.SaveOrderAsync(Order!);
            },
            Order!,
            AppResources.SaveOrderError);
    }
}