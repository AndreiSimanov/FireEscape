using CommunityToolkit.Maui.Core.Extensions;
using FireEscape.Resources.Languages;
using System.Collections.ObjectModel;
using Protocol = FireEscape.Models.Protocol;

namespace FireEscape.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        [ObservableProperty]
        public ObservableCollection<Protocol> protocols = new();

        readonly ProtocolService protocolService;
        readonly UserAccountService userAccountService;

        public MainViewModel(ProtocolService protocolService, UserAccountService userAccountService)
        {
            this.protocolService = protocolService;
            this.userAccountService = userAccountService;
        }

        public Protocol? SelectedProtocol { get; set; }

        [ObservableProperty]
        bool isRefreshing;

        [RelayCommand]
        async Task GetProtocolsAsync()
        {
            await DoCommand(async () =>
            {
                try
                {
                    var protocols = await protocolService.GetProtocolsAsync();
                    Protocols = protocols.ToObservableCollection();
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
            await DoCommand(async () =>
            {
                var protocol = await protocolService.CreateProtocolAsync();
                Protocols.Insert(0, protocol);
                SelectedProtocol = protocol;
            },
            AppResources.AddProtocolError);

            if (SelectedProtocol != null)
                await GoToDetailsAsync(SelectedProtocol);
        }

        [RelayCommand]
        async Task DeleteProtocol(Protocol protocol)
        {
            await DoCommand(async () =>
            {
                var action = await Shell.Current.DisplayActionSheet(AppResources.DeleteProtocol,
                    AppResources.Cancel,
                    AppResources.Delete);
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
                    await Shell.Current.DisplayAlert("",
                        string.Format(AppResources.UnregisteredApplicationMessage
                        , AppSettingsExtension.DeviceIdentifier), AppResources.OK);
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
                SelectedProtocol = protocol;
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
