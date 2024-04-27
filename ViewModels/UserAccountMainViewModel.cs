using System.Collections.ObjectModel;

namespace FireEscape.ViewModels;

public partial class UserAccountMainViewModel(UserAccountService userAccountService) : BaseViewModel
{
    [ObservableProperty]
    ObservableCollection<UserAccount> userAccounts = new();

    [ObservableProperty]
    bool isRefreshing;

    [ObservableProperty]
    bool isEmptyList = true;

    [ObservableProperty]
    string search = string.Empty;

    [ObservableProperty]
    string filter = string.Empty;

    [RelayCommand]
    async Task GetUserAccountsAsync() =>
        await DoCommandAsync(async () =>
        {
            try
            {
                IsRefreshing = false;
                UserAccounts.Clear();
                await foreach (var item in userAccountService.GetAsync())
                {
                    UserAccounts.Add(item);
                }
            }
            finally
            {
                IsRefreshing = false;
            }
        },
        AppResources.GetUserAccountsError);

    [RelayCommand]
    async Task GoToDetailsAsync(UserAccount userAccount) =>
        await DoCommandAsync(async () =>
        {
            await Shell.Current.GoToAsync(nameof(UserAccountPage), true,
                new Dictionary<string, object> { { nameof(UserAccountViewModel.EditObject), userAccount } });
        },
        userAccount,
        AppResources.EditUserAccountError);

    [RelayCommand]
    async Task DeleteUserAccountAsync(UserAccount userAccount) =>
        await DoCommandAsync(async () =>
        {
            var action = await Shell.Current.DisplayActionSheet(AppResources.DeleteUserAccount,
                AppResources.Cancel,
                AppResources.Delete);
            if (string.Equals(action, AppResources.Cancel))
                return;

            await userAccountService.DeleteAsync(userAccount);
            UserAccounts.Remove(userAccount);
        },
        userAccount,
        AppResources.DeleteUserAccountError);

    [RelayCommand]
    void FilterUserAccounts()
    {
        Filter = $"Contains([id], '{Search}') " +
            $"or Contains([Name], '{Search}') " +
            $"or Contains([Signature], '{Search}') " +
            $"or Contains([Company], '{Search}')";
    }
}
