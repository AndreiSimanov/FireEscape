namespace FireEscape.Views.BaseViews;

public abstract class BaseEditPage<T, M> : ContentPage where T : BaseEditViewModel<M>
{
    protected BaseEditPage(T viewModel) : base()
    {
        BindingContext = viewModel;
        Disappearing += ContentPageDisappearing!;
    }
    protected T? ViewModel => BindingContext as T;

    protected virtual async void ContentPageDisappearing(object sender, EventArgs e)
    {
        if (ViewModel != null)
            await ViewModel.SaveEditObjectCommand.ExecuteAsync(null);
    }
}
