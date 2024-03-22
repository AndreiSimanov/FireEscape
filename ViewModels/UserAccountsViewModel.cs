using System.Collections.ObjectModel;

namespace FireEscape.ViewModels
{
    public partial class UserAccountsViewModel : BaseViewModel
    {
        [ObservableProperty]
        public ObservableCollection<UserAccount> userAccountss = new();


        readonly UserAccountService userAccountService;

        public UserAccountsViewModel(UserAccountService userAccountService)
        {
            this.userAccountService = userAccountService;
        }

    }
}
