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
        UserAccountsViewModel?.GetUserAccountsCommand.Execute(null);
    }
}