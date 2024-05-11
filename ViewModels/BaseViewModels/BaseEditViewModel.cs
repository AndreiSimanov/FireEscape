using System.ComponentModel;
using System.Text.Json;

namespace FireEscape.ViewModels.BaseViewModels;

[QueryProperty(nameof(EditObject), nameof(EditObject))]
public abstract partial class BaseEditViewModel<T> : BaseViewModel
{
    [ObservableProperty]
    T? editObject;
    string? origObject;

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(EditObject))
            origObject = JsonSerializer.Serialize(EditObject);
    }

    [RelayCommand]
    async Task SaveEditObject()
    {
        if (EditObject == null)
            return;
        var editObject = JsonSerializer.Serialize(EditObject);
        if (!string.Equals(origObject, editObject))
            await SaveEditObjectAsync();
    }

    protected abstract Task SaveEditObjectAsync();
}