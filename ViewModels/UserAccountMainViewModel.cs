using FireEscape.Resources.Languages;
using System.Collections.ObjectModel;

namespace FireEscape.ViewModels
{
    public partial class UserAccountMainViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<UserAccount> userAccounts = new();

        readonly UserAccountService userAccountService;

        public UserAccountMainViewModel(UserAccountService userAccountService)
        {
            this.userAccountService = userAccountService;
        }

        [ObservableProperty]
        bool isRefreshing;

        [RelayCommand]
        async Task GetUserAccountsAsync()
        {
            await DoCommand(async () =>
            {
                try
                {
                    UserAccounts.Clear();
                    await foreach (var item in userAccountService.GetUserAccountsAsync())
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
        }

        [RelayCommand]
        async Task GoToDetailsAsync(UserAccount userAccount)
        {
            await DoCommand(async () =>
            {
                await Shell.Current.GoToAsync(nameof(UserAccountPage), true, new Dictionary<string, object> { { nameof(UserAccount), userAccount } });
            },
            userAccount,
            AppResources.EditUserAccountError);
        }

        [RelayCommand]
        async Task DeleteUserAccountAsync(UserAccount userAccount)
        {
            await DoCommand(async () =>
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
}
