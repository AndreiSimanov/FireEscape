namespace FireEscape.ViewModels
{
    [QueryProperty(nameof(Protocol), nameof(Protocol))]
    public partial class ProtocolViewModel : BaseViewModel
    {
        [ObservableProperty]
        Protocol? protocol;

        ProtocolService protocolService;

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
                await ProcessExeptionAsync("Unable to add photo to the protocol", ex);
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
                await ProcessExeptionAsync("Unable to save the protocol", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
