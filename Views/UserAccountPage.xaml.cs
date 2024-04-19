namespace FireEscape.Views;

public partial class UserAccountPage : BaseUserAccountPage<UserAccountViewModel>
{
    public UserAccountPage(UserAccountViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}