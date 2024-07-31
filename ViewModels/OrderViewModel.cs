namespace FireEscape.ViewModels;

public partial class OrderViewModel(OrderService orderService, ILogger<OrderViewModel> logger) : BaseEditViewModel<Order>(logger)
{
    protected override Task SaveEditObjectAsync() =>
       DoCommandAsync(() =>orderService.SaveAsync(EditObject!),
           EditObject,
           AppResources.SaveOrderError);
}