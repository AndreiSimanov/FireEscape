namespace FireEscape.Views;

public partial class UserAccountMainPage : ContentPage
{
    public UserAccountMainPage(UserAccountMainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    UserAccountMainViewModel? UserAccountMainViewModel => BindingContext as UserAccountMainViewModel;

    async void ContentPageAppearing(object sender, EventArgs e)
    {
        if (UserAccountMainViewModel != null && !UserAccountMainViewModel.UserAccounts.Any())
            await UserAccountMainViewModel.GetUserAccountsCommand.ExecuteAsync(null);
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

    async void ScrolledAsync(object sender, DevExpress.Maui.CollectionView.DXCollectionViewScrolledEventArgs e) => await searchControl.HideKeyboardAsync();
}