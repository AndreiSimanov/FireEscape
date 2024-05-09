namespace FireEscape.Views.BaseViews;

public abstract class BaseUserAccountPage : BaseEditPage<UserAccountViewModel, UserAccount>
{
    protected BaseUserAccountPage(UserAccountViewModel viewModel) : base(viewModel)
    {
    }
}
