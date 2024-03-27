using FireEscape.Resources.Languages;
using System.Collections.ObjectModel;
using Protocol = FireEscape.Models.Protocol;

namespace FireEscape.ViewModels
{
    public partial class ProtocolsMainViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<Protocol> protocols = new();

        readonly ProtocolService protocolService;
        readonly UserAccountService userAccountService;

        public ProtocolsMainViewModel(ProtocolService protocolService, UserAccountService userAccountService)
        {
            this.protocolService = protocolService;
            this.userAccountService = userAccountService;
        }

        [ObservableProperty]
        bool isRefreshing;

        [ObservableProperty]
        bool isEmptyList = true;

        [RelayCommand]
        async Task GetProtocolsAsync()
        {
            await DoCommand(async () =>
            {
                try
                {
                    if (Protocols.Any())
                        return;
                    await foreach (var item in protocolService.GetProtocolsAsync())
                    {
                        Protocols.Add(item);
                    }
                    IsEmptyList = !Protocols.Any();
                }
                finally
                {
                    IsRefreshing = false;
                }
            },
            AppResources.GetProtocolsError);
        }

        [RelayCommand]
        async Task AddProtocolAsync()
        {
            Protocol? newProtocol = null;
            await DoCommand(async () =>
            {
                newProtocol = await protocolService.CreateProtocolAsync();
                Protocols.Insert(0, newProtocol);
            },
            AppResources.AddProtocolError);

            if (newProtocol != null)
                await GoToDetailsAsync(newProtocol);
        }

        [RelayCommand]
        async Task DeleteProtocolAsync(Protocol protocol)
        {
            await DoCommand(async () =>
            {
                var action = await Shell.Current.DisplayActionSheet(AppResources.DeleteProtocol
                    , AppResources.Cancel
                    , AppResources.Delete);

                if (string.Equals(action, AppResources.Cancel))
                    return;

                await protocolService.DeleteProtocol(protocol);
                Protocols.Remove(protocol);
            },
            protocol,
            AppResources.DeleteProtocolError);
        }

        [RelayCommand]
        async Task CreateReportAsync(Protocol protocol)
        {
            await DoCommand(async () =>
            {
                var noExpiration = await userAccountService.CheckApplicationExpiration();
                if (noExpiration)
                {
                    var userAccount = await userAccountService.GetUserAccount();
                    await protocolService.CreateReportAsync(protocol, userAccount);
                }
                else
                {
                    await Shell.Current.DisplayAlert(""
                        , string.Format(AppResources.UnregisteredApplicationMessage
                        , AppSettingsExtension.UserAccountId), AppResources.OK);
                }
            },
            protocol,
            AppResources.CreateReportError);
        }

        [RelayCommand]
        async Task GoToDetailsAsync(Protocol protocol)
        {
            await DoCommand(async () =>
            {
                await Shell.Current.GoToAsync(nameof(ProtocolPage), true, new Dictionary<string, object> { { nameof(Protocol), protocol } });
                // await Shell.Current.GoToAsync($"//{nameof(ProtocolPage)}", true, new Dictionary<string, object> { { nameof(Protocol), protocol } });  //  modal form mode
            },
            protocol,
            AppResources.EditProtocolError);
        }

        [RelayCommand]
        async Task OpenUserAccountsPageAsync()
        {
            await DoCommand(async () =>
            {
                var userAccount = await userAccountService.DownloadUserAccountAsync();
                if (userAccountService.IsValidUserAccount(userAccount) && userAccount!.IsAdmin)
                {
                    await Shell.Current.GoToAsync(nameof(UserAccountsPage), true);
                }
            },
            AppResources.OpenUserAccountsPageError);
        }
    }
}
