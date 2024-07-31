namespace FireEscape.ViewModels;

public partial class UserAccountViewModel(UserAccountService userAccountService, ILogger<UserAccountViewModel> logger) : BaseEditViewModel<UserAccount>(logger)
{
    protected override Task SaveEditObjectAsync() =>
       DoCommandAsync(() => userAccountService.SaveAsync(EditObject!),
           EditObject,
           AppResources.SaveUserAccountError);
}