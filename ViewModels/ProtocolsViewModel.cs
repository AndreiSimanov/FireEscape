using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FireEscape.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Protocol = FireEscape.Models.Protocol;

namespace FireEscape.ViewModels
{
    public partial class ProtocolsViewModel: BaseViewModel
    {

        public ObservableCollection<Protocol> Protocols { get; private set; } = new();
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
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var protocols = await protocolService.GetProtocols();

                if (Protocols.Count != 0)
                    Protocols.Clear();


                //!! Protocols = protocols.ToObservableCollection();

                foreach (var protocol in protocols)
                    Protocols.Add(protocol);

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
