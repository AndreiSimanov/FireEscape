using FireEscape.Resources.Languages;
using System.Diagnostics;

namespace FireEscape.ViewModels
{
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
            await Shell.Current.DisplayAlert(AppResources.Error, ex.Message, AppResources.Ok);
        }
    }
}
