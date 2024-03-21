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
            await DoCommand(async () =>
            {
                await protocolService.AddProtocolPhotoAsync(Protocol!);
                OnPropertyChanged(nameof(Protocol));

            },
            Protocol!,
            AppResources.AddPhotoError);
        }


        [RelayCommand]
        async Task SaveProtocolAsync()
        {
            await DoCommand(async () =>
            {
                await protocolService.SaveProtocolAsync(Protocol!);
            },
            Protocol!,
            AppResources.SaveProtocolError);
        }


        [RelayCommand]
        async Task GoToDetailsAsync()
        {
            await DoCommand(async () =>
            {
                await Shell.Current.GoToAsync(nameof(FireEscapePage), true, new Dictionary<string, object> { { nameof(FireEscape), Protocol!.FireEscape } });
            },
            Protocol!,
            AppResources.EditFireEscapeError);
        }
    }
}
