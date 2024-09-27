namespace FireEscape.ViewModels;

public partial class ProtocolViewModel(IProtocolService protocolService, ILogger<ProtocolViewModel> logger) : BaseEditViewModel<Protocol>(logger)
{
    [RelayCommand]
    Task AddPhotoAsync() => 
        DoBusyCommandAsync(() => protocolService.AddPhotoAsync(EditObject!),
            EditObject,
            AppResources.AddPhotoError);

    [RelayCommand]
    Task SelectPhotoAsync() =>
        DoBusyCommandAsync(() => protocolService.SelectPhotoAsync(EditObject!),
            EditObject,
            AppResources.AddPhotoError);

    [RelayCommand]
    Task GoToDetailsAsync() =>
        DoBusyCommandAsync(() => Shell.Current.GoToAsync(nameof(StairsPage), true,
            new Dictionary<string, object> { { nameof(StairsViewModel.EditObject), EditObject!.Stairs } }),
            EditObject,
            AppResources.EditStairsError);

    protected override Task SaveEditObjectAsync() =>
       DoCommandAsync(() => protocolService.SaveAsync(EditObject!),
           EditObject,
           AppResources.SaveProtocolError);

}