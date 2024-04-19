namespace FireEscape.ViewModels;

public partial class UserAccountViewModel(UserAccountService userAccountService) : BaseEditViewModel<UserAccount>
{
    protected override async Task SaveEditObjectAsync() =>
       await DoCommandAsync(async () =>
       {
           await userAccountService.SaveUserAccountAsync(EditObject!);
       },
       EditObject!,
       AppResources.SaveUserAccountError);
}