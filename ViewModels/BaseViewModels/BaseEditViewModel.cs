using System.ComponentModel;
using System.Text.Json;

namespace FireEscape.ViewModels.BaseViewModels;

[QueryProperty(nameof(EditObject), nameof(EditObject))]
public abstract partial class BaseEditViewModel<T>(ILogger<BaseEditViewModel<T>> logger) : BaseViewModel(logger)
{
    [ObservableProperty]
    T? editObject;
    string? origObject;

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(EditObject))
            SetEditObject(EditObject);
    }

    [RelayCommand]
    async Task SaveEditObject()
    {
        if (EditObject == null)
            return;
        var editObject = JsonSerializer.Serialize(EditObject);
        if (!string.Equals(origObject, editObject))
            await SaveEditObjectAsync();
        SetEditObject(EditObject);
    }

    void SetEditObject(T? editObject)
    {
        if (editObject != null)
            origObject = JsonSerializer.Serialize(editObject);
        else
            origObject = null;
    }

    protected abstract Task SaveEditObjectAsync();
}