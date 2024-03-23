using CommunityToolkit.Maui.Core.Extensions;

namespace FireEscape.Views;

public partial class UserAccountsPage : ContentPage
{
    public UserAccountsPage(UserAccountsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private UserAccountsViewModel? UserAccountsViewModel
    {
        get
        {
            return BindingContext as UserAccountsViewModel;
        }
    }

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        if (UserAccountsViewModel != null && !UserAccountsViewModel.UserAccounts.Any())
            UserAccountsViewModel.GetUserAccountsCommand.Execute(null);
    }
}