namespace FireEscape.ViewModels;

public partial class ProtocolViewModel(ProtocolService protocolService, ILogger<ProtocolViewModel> logger) : BaseEditViewModel<Protocol>(logger)
{
    [RelayCommand]
    async Task AddPhotoAsync() =>
        await DoBusyCommandAsync(async () =>
        {
            await protocolService.AddPhotoAsync(EditObject!);
        },
        EditObject,
        AppResources.AddPhotoError);

    [RelayCommand]
    async Task SelectPhotoAsync() =>
        await DoBusyCommandAsync(async () =>
        {
            await protocolService.SelectPhotoAsync(EditObject!);
        },
        EditObject,
        AppResources.AddPhotoError);

    [RelayCommand]
    async Task GoToDetailsAsync()
    {
        await SaveEditObjectCommand.ExecuteAsync(null);

        await DoBusyCommandAsync(async () =>
        {
            await Shell.Current.GoToAsync(nameof(StairsPage), true,
                new Dictionary<string, object> { { nameof(StairsViewModel.EditObject), EditObject!.Stairs } });
        },
        EditObject,
        AppResources.EditStairsError);
    }

    protected override async Task SaveEditObjectAsync() =>
       await DoBusyCommandAsync(async () =>
       {
           await protocolService.SaveAsync(EditObject!);
       },
       EditObject,
       AppResources.SaveProtocolError);
}