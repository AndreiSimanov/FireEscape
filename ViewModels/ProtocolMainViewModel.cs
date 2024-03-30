using CommunityToolkit.Maui.Core.Extensions;
using FireEscape.Resources.Languages;
using System.Collections.ObjectModel;
using Protocol = FireEscape.Models.Protocol;

namespace FireEscape.ViewModels
{
    public partial class ProtocolMainViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<Protocol> protocols = new();

        readonly ProtocolService protocolService;
        readonly UserAccountService userAccountService;

        public ProtocolMainViewModel(ProtocolService protocolService, UserAccountService userAccountService)
        {
            this.protocolService = protocolService;
            this.userAccountService = userAccountService;
        }

        [ObservableProperty]
        bool isRefreshing;

        [ObservableProperty]
        bool isEmptyList = true;

        [RelayCommand]
        void GetProtocolsSync() //Sync version works faster for local drive
        {
            DoCommand(() =>
            {
                try
                {
                    if (Protocols.Any())
                        return;
                    IsRefreshing = true;
                    var protocols = protocolService.GetProtocols();
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
        async Task GetProtocolsAsync()
        {
            await DoCommandAsync(async () =>
            {
                try
                {
                    if (Protocols.Any())
                        return;
                    IsRefreshing = true;
                    var protocols = new List<Protocol>();
                    await foreach (var item in protocolService.GetProtocolsAsync())
                    {
                        protocols.Add(item);
                    }
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
            Protocol? newProtocol = null;
            await DoCommandAsync(async () =>
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
            await DoCommandAsync(async () =>
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
            await DoCommandAsync(async () =>
            {
                var noExpiration = await userAccountService.CheckApplicationExpiration();
                if (noExpiration)
                {
                    var userAccount = await userAccountService.GetUserAccount();
                    if (userAccount != null)
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
            await DoCommandAsync(async () =>
            {
                await Shell.Current.GoToAsync(nameof(ProtocolPage), true, new Dictionary<string, object> { { nameof(Protocol), protocol } });
                // await Shell.Current.GoToAsync($"//{nameof(ProtocolPage)}", true, new Dictionary<string, object> { { nameof(Protocol), protocol } });  //  modal form mode
            },
            protocol,
            AppResources.EditProtocolError);
        }

        [RelayCommand]
        async Task OpenUserAccountMainPageAsync()
        {
            await DoCommandAsync(async () =>
            {
                var userAccount = await userAccountService.DownloadUserAccountAsync();
                if (userAccountService.IsValidUserAccount(userAccount) && userAccount!.IsAdmin)
                {
                    await Shell.Current.GoToAsync(nameof(UserAccountMainPage), true);
                }
            },
            AppResources.OpenUserAccountMainPageError);
        }

        [RelayCommand] //For test only 
        async Task AddProtocolsAsync()
        {
            await DoCommandAsync(async () =>
            {
                for (var i = 0; i < 1000; i++)
                {
                    await protocolService.CreateProtocolAsync();
                }
            },
            AppResources.AddProtocolError);
        }
    }
}
