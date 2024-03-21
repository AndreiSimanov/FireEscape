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

        public MainViewModel(ProtocolService protocolService)
        {
            this.protocolService = protocolService;
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
                await protocolService.CreateReportAsync(protocol);
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
    }
}
