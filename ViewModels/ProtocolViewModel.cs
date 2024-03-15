using FireEscape.Resources.Languages;

namespace FireEscape.ViewModels
{
    [QueryProperty(nameof(Protocol), nameof(Protocol))]
    public partial class ProtocolViewModel : BaseViewModel
    {
        [ObservableProperty]
        Protocol? protocol;

        readonly ProtocolService protocolService;

        public ProtocolViewModel(ProtocolService protocolService)
        {
            this.protocolService = protocolService;
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
