namespace FireEscape.Views;

public partial class UserAccountMainPage : ContentPage
{
    public UserAccountMainPage(UserAccountMainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private UserAccountMainViewModel? UserAccountsViewModel => BindingContext as UserAccountMainViewModel;

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        if (UserAccountsViewModel != null && !UserAccountsViewModel.UserAccounts.Any())
            UserAccountsViewModel.GetUserAccountsCommand.Execute(null);
    }
}