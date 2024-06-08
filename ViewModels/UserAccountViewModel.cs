namespace FireEscape.ViewModels;

public partial class UserAccountViewModel(UserAccountService userAccountService, ILogger<UserAccountViewModel> logger) : BaseEditViewModel<UserAccount>(logger)
{
    protected override async Task SaveEditObjectAsync() =>
       await DoCommandAsync(async () =>
       {
           await userAccountService.SaveAsync(EditObject!);
       },
       EditObject,
       AppResources.SaveUserAccountError);
}