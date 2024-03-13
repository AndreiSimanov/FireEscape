using CommunityToolkit.Maui.Core.Extensions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Protocol = FireEscape.Models.Protocol;

namespace FireEscape.ViewModels
{
    public partial class ProtocolsViewModel : BaseViewModel
    {
        [ObservableProperty]
        public ObservableCollection<Protocol> protocols = new();
        ProtocolService protocolService;


        public ProtocolsViewModel(ProtocolService protocolService)
        {
            Title = "Fire Escape";
            this.protocolService = protocolService;
        }

        [ObservableProperty]
        bool isRefreshing;

        [RelayCommand]
        async Task GetProtocolsAsync()
        {
            if (IsBusy || Protocols?.Count != 0)
                return;

            try
            {
                IsBusy = true;
                var protocols = await protocolService.GetProtocols();
                Protocols = protocols.ToObservableCollection();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get protocols: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }
    }
}
