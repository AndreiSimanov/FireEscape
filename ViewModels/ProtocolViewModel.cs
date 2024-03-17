using FireEscape.Resources.Languages;
using Microsoft.Extensions.Options;

namespace FireEscape.ViewModels
{
    [QueryProperty(nameof(Protocol), nameof(Protocol))]
    public partial class ProtocolViewModel : BaseViewModel
    {
        [ObservableProperty]
        Protocol? protocol;

        readonly ProtocolService protocolService;
        public ProtocolPropertiesDictionary ProtocolPropertiesDictionary { get; private set; }

        public ProtocolViewModel(ProtocolService protocolService, IOptions<ProtocolPropertiesDictionary> protocolPropertiesDictionary)
        {
            this.protocolService = protocolService;
            ProtocolPropertiesDictionary = protocolPropertiesDictionary.Value;
        }

        [RelayCommand]
        async Task AddProtocolPhotoAsync()
        {
            if (IsBusy || Protocol == null)
                return;
            try
            {
                await protocolService.AddProtocolPhotoAsync(Protocol);
                OnPropertyChanged(nameof(Protocol));
            }
            catch (Exception ex)
            {
                await ProcessExeptionAsync(AppResources.AddPhotoError, ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task SaveProtocolAsync()
        {
            if (IsBusy || Protocol == null)
                return;
            try
            {
                await protocolService.SaveProtocolAsync(Protocol);
            }
            catch (Exception ex)
            {
                await ProcessExeptionAsync(AppResources.SaveProtocolError, ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
