using FireEscape.Resources.Languages;

namespace FireEscape.ViewModels
{
    [QueryProperty(nameof(UserAccount), nameof(UserAccount))]
    public partial class UserAccountViewModel : BaseViewModel
    {
        readonly UserAccountService userAccountService;

        public UserAccountViewModel(UserAccountService userAccountService)
        {
            this.userAccountService = userAccountService;
        }

        [ObservableProperty]
        UserAccount? userAccount;

        [RelayCommand]
        async Task SaveUserAccountAsync()
        {
            await DoCommand(async () =>
            {
                await userAccountService.SaveUserAccountAsync(UserAccount!);
            },
            UserAccount!,
            AppResources.SaveProtocolError);
        }
    }
}
