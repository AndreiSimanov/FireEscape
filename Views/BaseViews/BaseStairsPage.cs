namespace FireEscape.Views.BaseViews;

public abstract class BaseStairsPage<T> : BaseEditPage<BaseEditViewModel<Stairs>, Stairs>
{
    protected BaseStairsPage(BaseEditViewModel<Stairs> viewModel) : base(viewModel)
    {
    }
}