using FireEscape.Resources.Languages;
using System.Collections.ObjectModel;

namespace FireEscape.ViewModels
{
    public partial class UserAccountsViewModel : BaseViewModel
    {
        [ObservableProperty]
        public ObservableCollection<UserAccount> userAccounts = new();

        readonly UserAccountService userAccountService;

        public UserAccountsViewModel(UserAccountService userAccountService)
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
    }
}
