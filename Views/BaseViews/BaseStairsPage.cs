namespace FireEscape.Views.BaseViews;

public abstract class BaseStairsPage : BaseEditPage<StairsViewModel, Stairs>
{
    protected BaseStairsPage(StairsViewModel viewModel) : base(viewModel)
    {
    }
}