using FireEscape.Resources.Languages;
using System.ComponentModel;

namespace FireEscape.ViewModels;

[QueryProperty(nameof(Order), nameof(Order))]
public partial class OrderViewModel(OrderService orderService) : BaseViewModel
{
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
    }

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