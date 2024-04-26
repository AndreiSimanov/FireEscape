namespace FireEscape.Views.BaseViews;

public abstract class BaseProtocolPage : BaseEditPage<ProtocolViewModel, Protocol>
{
    protected BaseProtocolPage(ProtocolViewModel viewModel) : base(viewModel)
    {
    }
}