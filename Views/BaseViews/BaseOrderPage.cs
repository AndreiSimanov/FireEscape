namespace FireEscape.Views.BaseViews;

public abstract class BaseOrderPage : BaseEditPage<OrderViewModel, Order>
{
    protected BaseOrderPage(OrderViewModel viewModel) : base(viewModel)
    {
    }
}