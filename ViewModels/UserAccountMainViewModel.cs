using FireEscape.Resources.Languages;
using System.Collections.ObjectModel;

namespace FireEscape.ViewModels
{
    public partial class UserAccountMainViewModel(UserAccountService userAccountService) : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<UserAccount> userAccounts = new();
        [ObservableProperty]
        bool isRefreshing;

        [ObservableProperty]
        bool isEmptyList = true;

        [RelayCommand]
        async Task GetUserAccountsAsync() =>
            await DoCommandAsync(async () =>
            {
                try
                {
                    IsRefreshing = false;
                    UserAccounts.Clear();
                    await foreach (var item in userAccountService.GetUserAccountsAsync())
                    {
                        UserAccounts.Add(item);
                    }
                    IsEmptyList = !UserAccounts.Any();
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
                await Shell.Current.GoToAsync(nameof(UserAccountPage), true, new Dictionary<string, object> { { nameof(UserAccount), userAccount } });
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

                await userAccountService.DeleteUserAccount(userAccount);
                UserAccounts.Remove(userAccount);
            },
            userAccount,
            AppResources.DeleteUserAccountError);
    }
}
