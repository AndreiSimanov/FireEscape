using FireEscape.Resources.Languages;

namespace FireEscape.ViewModels;

[QueryProperty(nameof(Protocol), nameof(Protocol))]
public partial class ProtocolViewModel(ProtocolService protocolService) : BaseViewModel
{
    [ObservableProperty]
    Protocol? protocol;

    [RelayCommand]
    async Task AddPhotoAsync() =>
        await DoCommandAsync(async () =>
        {
            await protocolService.AddPhotoAsync(Protocol!);
        },
        Protocol!,
        AppResources.AddPhotoError);

    [RelayCommand]
    async Task SelectPhotoAsync() =>
        await DoCommandAsync(async () =>
        {
            await protocolService.SelectPhotoAsync(Protocol!);
        },
        Protocol!,
        AppResources.AddPhotoError);

    [RelayCommand]
    async Task SaveProtocolAsync() =>
        await DoCommandAsync(async () =>
        {
            await protocolService.SaveProtocolAsync(Protocol!);
        },
        Protocol!,
        AppResources.SaveProtocolError);

    [RelayCommand]
    async Task GoToDetailsAsync() =>
        await DoCommandAsync(async () =>
        {
            await Shell.Current.GoToAsync(nameof(StairsPage), true, new Dictionary<string, object> { { nameof(Stairs), Protocol!.Stairs } });
        },
        Protocol!,
        AppResources.EditStairsError);
}