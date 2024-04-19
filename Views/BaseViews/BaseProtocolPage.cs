namespace FireEscape.Views.BaseViews;

public abstract class BaseProtocolPage<T> : BaseEditPage<BaseEditViewModel<Protocol>, Protocol>
{
    protected BaseProtocolPage(BaseEditViewModel<Protocol> viewModel) : base(viewModel)
    {
    }
}