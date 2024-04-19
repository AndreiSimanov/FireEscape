using FireEscape.Resources.Languages;

namespace FireEscape.ViewModels;

[QueryProperty(nameof(UserAccount), nameof(UserAccount))]
public partial class UserAccountViewModel(UserAccountService userAccountService) : BaseViewModel
{
    [ObservableProperty]
    UserAccount? userAccount;

    [RelayCommand]
    async Task SaveUserAccountAsync() =>
        await DoCommandAsync(async () =>
        {
            await userAccountService.SaveUserAccountAsync(UserAccount!);
        },
        UserAccount!,
        AppResources.SaveProtocolError);
}