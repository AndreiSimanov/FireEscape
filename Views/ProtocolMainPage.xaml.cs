namespace FireEscape.Views;
public partial class ProtocolMainPage : ContentPage
{
    public ProtocolMainPage(ProtocolMainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    ProtocolMainViewModel? ProtocolMainViewModel => BindingContext as ProtocolMainViewModel;

    async void ContentPageAppearing(object sender, EventArgs e)
    {
        if (ProtocolMainViewModel != null)
            await ProtocolMainViewModel.GetProtocolsCommand.ExecuteAsync(null);
    }

    async void CreateProtocol(object sender, EventArgs e)
    {
        if (ProtocolMainViewModel != null)
        {
            await ProtocolMainViewModel.AddProtocolCommand.ExecuteAsync(null);
            protocols.ScrollTo(0);
        }
    }

    public void CopyProtocolWithStairs(object sender, DevExpress.Maui.CollectionView.SwipeItemTapEventArgs e)
    {
        if (e.Item is not Protocol protocol)
            return;
        ProtocolMainViewModel?.CopyProtocolWithStairsCommand.Execute(protocol);
        protocols.ScrollTo(0);
    }

    void CollectionViewChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (ProtocolMainViewModel != null)
            ProtocolMainViewModel.IsEmptyList = protocols.VisibleItemCount == 0 && !ProtocolMainViewModel.IsRefreshing;
    }

    async void ScrolledAsync(object sender, DevExpress.Maui.CollectionView.DXCollectionViewScrolledEventArgs e) => await searchControl.HideKeyboardAsync();
}