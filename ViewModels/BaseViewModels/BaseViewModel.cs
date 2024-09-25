using System.Diagnostics;

namespace FireEscape.ViewModels.BaseViewModels;

public partial class BaseViewModel(ILogger<BaseViewModel> logger) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    [ObservableProperty]
    string? title;

    public bool IsNotBusy => !IsBusy;

    protected Task ProcessExeptionAsync(string caption, Exception ex)
    {
        Debug.WriteLine($"{caption}: {ex.Message}");
        logger.LogError(ex, message: ex.Message);
        return Shell.Current.DisplayAlert(AppResources.Error, ex.Message, AppResources.OK);
    }

    protected Task DoBusyCommandAsync(Func<Task> func, object? item, string exceptionCaption)
    {
        if (item != null)
            return DoBusyCommandAsync(func, exceptionCaption);
        return Task.CompletedTask;
    }

    protected async Task DoBusyCommandAsync(Func<Task> func, string exceptionCaption)
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

    protected Task DoCommandAsync(Func<Task> func, object? item, string exceptionCaption)
    {
        if (item != null)
            return DoCommandAsync(func, exceptionCaption);
        return Task.CompletedTask;
    }

    protected async Task DoCommandAsync(Func<Task> func, string exceptionCaption)
    {
        try
        {
            await func();
        }
        catch (Exception ex)
        {
            await ProcessExeptionAsync(exceptionCaption, ex);
        }
    }

    protected void DoBusyCommand(Action action, string exceptionCaption)
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
            ProcessExeptionAsync(exceptionCaption, ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
    protected void DoCommand(Action action, string exceptionCaption)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            ProcessExeptionAsync(exceptionCaption, ex);
        }
    }
}