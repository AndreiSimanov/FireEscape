using FireEscape.Resources.Languages;
using System.ComponentModel;
using System.Text.Json;

namespace FireEscape.ViewModels;

[QueryProperty(nameof(Protocol), nameof(Protocol))]
public partial class ProtocolViewModel(ProtocolService protocolService) : BaseViewModel
{
    [ObservableProperty]
    Protocol? protocol;
    string? origProtocol;

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
            if (Protocol == null)
                return;
            var editedProtocol = JsonSerializer.Serialize(Protocol);
            if (!string.Equals(origProtocol, editedProtocol))
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

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(Protocol))
            origProtocol = JsonSerializer.Serialize(Protocol);
    }
}