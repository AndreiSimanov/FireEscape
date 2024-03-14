using CommunityToolkit.Maui.Core.Extensions;
using System.Collections.ObjectModel;
using Protocol = FireEscape.Models.Protocol;

namespace FireEscape.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        [ObservableProperty]
        public ObservableCollection<Protocol> protocols = new();

        ProtocolService protocolService;

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
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var protocols = await protocolService.GetProtocolsAsync();
                Protocols = protocols.ToObservableCollection();
            }
            catch (Exception ex)
            {
                await ProcessExeptionAsync("Unable to get protocols", ex);
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        async Task AddProtocolAsync()
        {
            if (IsBusy)
                return;
            try
            {
                var protocol = new Protocol();
                Protocols.Insert(0, protocol);
                await protocolService.SaveProtocolAsync(protocol);
                SelectedProtocol = protocol;
                await GoToDetailsAsync(protocol);
            }
            catch (Exception ex)
            {
                await ProcessExeptionAsync("Unable to add protocol", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task DeleteProtocol(Protocol protocol)
        {
            if (IsBusy || protocol == null)
                return;

            var action = await Shell.Current.DisplayActionSheet(AppRes.Get<string>(AppRes.DELETE_PROTOCOL), 
                AppRes.Get<string>(AppRes.CANCEL), 
                AppRes.Get<string>(AppRes.DELETE));
            if (string.Equals(action, AppRes.Get<string>(AppRes.CANCEL)))
                return;
            try
            {
                protocolService.DeleteProtocol(protocol);
                Protocols.Remove(protocol);
            }
            catch (Exception ex)
            {
                await ProcessExeptionAsync("Unable to delete protocol", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task CreatePdfAsync(Protocol protocol)
        {
            if (IsBusy || protocol == null)
                return;

            try
            {
                await protocolService.CreatePdfAsync(protocol);
            }
            catch (Exception ex)
            {
                await ProcessExeptionAsync("Unable to create PDF file", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }


        [RelayCommand]
        async Task GoToDetailsAsync(Protocol protocol)
        {
            if (protocol == null)
                return;
            SelectedProtocol = protocol;
            await Shell.Current.GoToAsync(nameof(ProtocolPage), true, new Dictionary<string, object> { { nameof(Protocol), protocol } });
            //  await Shell.Current.GoToAsync($"//{nameof(ProtocolPage)}", true, new Dictionary<string, object> { { nameof(Protocol), protocol } });  //  modal form mode
        }
    }
}
