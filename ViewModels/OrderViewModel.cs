using FireEscape.Resources.Languages;
using System.ComponentModel;
using System.Text.Json;

namespace FireEscape.ViewModels;

[QueryProperty(nameof(Order), nameof(Order))]
public partial class OrderViewModel(OrderService orderService) : BaseViewModel
{
    [ObservableProperty]
    Order? order;
    string? origOrder;

    [RelayCommand]
    async Task SaveOrderAsync() =>
        await DoCommandAsync(async () =>
        {
            if (Order == null)
                return;
            var editedOrder = JsonSerializer.Serialize(Order);
            if (!string.Equals(origOrder, editedOrder))
                await orderService.SaveOrderAsync(Order);
        },
        Order!,
        AppResources.SaveOrderError);

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(Order))
            origOrder = JsonSerializer.Serialize(Order);
    }
}