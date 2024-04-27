namespace FireEscape.Views;

public partial class UserAccountMainPage : ContentPage
{
    public UserAccountMainPage(UserAccountMainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    UserAccountMainViewModel? UserAccountMainViewModel => BindingContext as UserAccountMainViewModel;

    void ContentPage_Appearing(object sender, EventArgs e)
    {
        if (UserAccountMainViewModel != null && !UserAccountMainViewModel.UserAccounts.Any())
            userAccounts.PullToRefreshCommand.Execute(null);
    }

    void CreateUserAccount(object sender, EventArgs e)
    {
        //todo: make UserAccount
    }

    void CollectionViewChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (UserAccountMainViewModel != null)
            UserAccountMainViewModel.IsEmptyList = userAccounts.VisibleItemCount == 0;
    }
}