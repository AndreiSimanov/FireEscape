using System.ComponentModel;
using System.Text.Json;

namespace FireEscape.ViewModels.BaseViewModels;

[QueryProperty(nameof(EditObject), nameof(EditObject))]
public abstract partial class BaseEditViewModel<T>(ILogger<BaseEditViewModel<T>> logger) : BaseViewModel(logger)
{
    [ObservableProperty]
    T? editObject;
    string? origObject;

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

    [RelayCommand]
    async Task ValidateEditObjectAsync()
    {
        var validationResult = GetEditObjectValidationResult();
        if (validationResult.Count == 0)
        {
            await Shell.Current.Navigation.PopAsync();
            return;
        }
        var action = await Shell.Current.DisplayActionSheet(
            AppResources.ValidationError, AppResources.Edit, AppResources.Exit, string.Join(Environment.NewLine, validationResult));
        if (string.Equals(action, AppResources.Exit))
            await Shell.Current.Navigation.PopAsync();
    }

    void SetEditObject(T? editObject)
    {
        if (editObject != null)
            origObject = JsonSerializer.Serialize(editObject);
        else
            origObject = null;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(EditObject))
            SetEditObject(EditObject);
    }

    protected abstract Task SaveEditObjectAsync();

    protected virtual List<string> GetEditObjectValidationResult() => new();
}