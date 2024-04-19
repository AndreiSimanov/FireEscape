namespace FireEscape.Views.BaseViews;

public abstract class BaseOrderPage<T> : BaseEditPage<BaseEditViewModel<Order>, Order>
{
    protected BaseOrderPage(BaseEditViewModel<Order> viewModel) : base(viewModel)
    {
    }
}