using DevExpress.Maui.Editors;

namespace FireEscape.Views;
public partial class ProtocolMainPage : ContentPage
{
    public ProtocolMainPage(ProtocolMainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    ProtocolMainViewModel? ProtocolMainViewModel => BindingContext as ProtocolMainViewModel;

    void ContentPage_Appearing(object sender, EventArgs e)
    {
        protocols.PullToRefreshCommand.Execute(null);
    }

    void CreateProtocol(object sender, EventArgs e)
    {
        ProtocolMainViewModel?.AddProtocolCommand.Execute(null);
        protocols.ScrollTo(0);
    }

    void CopyProtocol(object sender, DevExpress.Maui.CollectionView.SwipeItemTapEventArgs e)
    {
        var protocol = e.Item as Protocol;
        if (protocol == null)
            return;
        ProtocolMainViewModel?.CopyProtocolCommand.Execute(protocol);
        protocols.ScrollTo(0);
    }

    void CollectionViewChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (ProtocolMainViewModel != null)
            ProtocolMainViewModel.IsEmptyList = protocols.VisibleItemCount == 0 && !ProtocolMainViewModel.IsRefreshing;
    }
}