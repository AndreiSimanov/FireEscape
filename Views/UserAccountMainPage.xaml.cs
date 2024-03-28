using DevExpress.Maui.Editors;

namespace FireEscape.Views;

public partial class UserAccountMainPage : ContentPage
{
    public UserAccountMainPage(UserAccountMainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private UserAccountMainViewModel? UserAccountMainViewModel => BindingContext as UserAccountMainViewModel;

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        if (UserAccountMainViewModel != null && !UserAccountMainViewModel.UserAccounts.Any())
            userAccounts.PullToRefreshCommand.Execute(null);    
    }

    private void CreateUserAccount(object sender, EventArgs e)
    {
        //todo: make UserAccount

    }
    void SearchTextChanged(object sender, EventArgs e)
    {
        var searchText = ((TextEdit)sender).Text;
        userAccounts.FilterString = $"Contains([id], '{searchText}') " +
            $"or Contains([Name], '{searchText}') " +
            $"or Contains([Signature], '{searchText}') " +
            $"or Contains([Company], '{searchText}')";
    }

    private void CollectionViewChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (UserAccountMainViewModel != null)
            UserAccountMainViewModel.IsEmptyList = userAccounts.VisibleItemCount == 0;
    }
}