using System.Collections.ObjectModel;

namespace FireEscape.ViewModels;

public partial class UserAccountMainViewModel(UserAccountService userAccountService, ILogger<UserAccountMainViewModel> logger) : BaseViewModel(logger)
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

    [ObservableProperty]
    object? selectedItem = null;

    [RelayCommand]
    async Task GetUserAccountsAsync() =>
        await DoBusyCommandAsync(async () =>
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
        await DoBusyCommandAsync(async () =>
        {
            SelectedItem = userAccount;
            await Shell.Current.GoToAsync(nameof(UserAccountPage), true,
                new Dictionary<string, object> { { nameof(UserAccountViewModel.EditObject), userAccount } });
        },
        userAccount,
        AppResources.EditUserAccountError);

    [RelayCommand]
    async Task DeleteUserAccountAsync(UserAccount userAccount) =>
        await DoBusyCommandAsync(async () =>
        {
            SelectedItem = userAccount;
            var action = await Shell.Current.DisplayActionSheet(AppResources.DeleteUserAccount,
                AppResources.Cancel,
                AppResources.Delete);
            if (string.Equals(action, AppResources.Cancel))
                return;

            await userAccountService.DeleteAsync(userAccount);
            UserAccounts.Remove(userAccount);
            SelectedItem = null;
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
