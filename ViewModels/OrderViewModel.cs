namespace FireEscape.ViewModels;

public partial class OrderViewModel(OrderService orderService, ILogger<OrderViewModel> logger) : BaseEditViewModel<Order>(logger)
{
    protected override async Task SaveEditObjectAsync() =>
       await DoBusyCommandAsync(async () =>
       {
           await orderService.SaveAsync(EditObject!);
       },
       EditObject,
       AppResources.SaveOrderError);
}