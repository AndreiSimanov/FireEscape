using FireEscape.Resources.Languages;
using System.Diagnostics;

namespace FireEscape.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    [ObservableProperty]
    string? title;

    public bool IsNotBusy => !IsBusy;

    protected async Task ProcessExeptionAsync(string caption, Exception ex)
    {
        Debug.WriteLine($"{caption}: {ex.Message}");
        await Shell.Current.DisplayAlert(AppResources.Error, ex.Message, AppResources.OK);
    }

    protected async Task DoCommandAsync(Func<Task> func, object item, string exceptionCaption)
    {
        if (item != null)
            await DoCommandAsync(func, exceptionCaption);
    }

    protected async Task DoCommandAsync(Func<Task> func, string exceptionCaption)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            await func();
        }
        catch (Exception ex)
        {
            await ProcessExeptionAsync(exceptionCaption, ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected void DoCommand(Action action, string exceptionCaption)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            action();
        }
        catch (Exception ex)
        {
            _ = ProcessExeptionAsync(exceptionCaption, ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}