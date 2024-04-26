namespace FireEscape.ViewModels;

public partial class OrderViewModel(OrderService orderService) : BaseEditViewModel<Order>
{
    protected override async Task SaveEditObjectAsync() =>
       await DoCommandAsync(async () =>
       {
           await orderService.SaveAsync(EditObject!);
       },
       EditObject,
       AppResources.SaveOrderError);
}