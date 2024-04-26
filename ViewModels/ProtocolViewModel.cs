namespace FireEscape.ViewModels;

public partial class ProtocolViewModel(ProtocolService protocolService) : BaseEditViewModel<Protocol>
{
    [RelayCommand]
    async Task AddPhotoAsync() =>
        await DoCommandAsync(async () =>
        {
            await protocolService.AddPhotoAsync(EditObject!);
        },
        EditObject,
        AppResources.AddPhotoError);

    [RelayCommand]
    async Task SelectPhotoAsync() =>
        await DoCommandAsync(async () =>
        {
            await protocolService.SelectPhotoAsync(EditObject!);
        },
        EditObject,
        AppResources.AddPhotoError);

    [RelayCommand]
    async Task GoToDetailsAsync() =>
        await DoCommandAsync(async () =>
        {
            await Shell.Current.GoToAsync(nameof(StairsPage), true,
                new Dictionary<string, object> { { nameof(StairsViewModel.EditObject), EditObject!.Stairs! } });
        },
        EditObject,
        AppResources.EditStairsError);

    protected override async Task SaveEditObjectAsync() =>
       await DoCommandAsync(async () =>
       {
           await protocolService.SaveProtocolAsync(EditObject!);
       },
       EditObject,
       AppResources.SaveProtocolError);
}