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
                await ProcessExeptionAsync(AppResources.GetProtocolsError, ex);
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
                var protocol = await protocolService.CreateProtocol();
                Protocols.Insert(0, protocol);
                SelectedProtocol = protocol;
                await GoToDetailsAsync(protocol);
            }
            catch (Exception ex)
            {
                await ProcessExeptionAsync(AppResources.AddProtocolError, ex);
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

            var action = await Shell.Current.DisplayActionSheet(AppResources.DeleteProtocol, 
                AppResources.Cancel, 
                AppResources.Delete);
            if (string.Equals(action, AppResources.Cancel))
                return;
            try
            {
                protocolService.DeleteProtocol(protocol);
                Protocols.Remove(protocol);
            }
            catch (Exception ex)
            {
                await ProcessExeptionAsync(AppResources.DeleteProtocolError, ex);
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
                await ProcessExeptionAsync(AppResources.CreatePdfFileError, ex);
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
