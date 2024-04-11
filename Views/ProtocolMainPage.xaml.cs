using DevExpress.Maui.Editors;
using System.Diagnostics;

namespace FireEscape.Views;

public partial class ProtocolMainPage : ContentPage
{
    Stopwatch tapStopwatch = new();

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

    void SearchTextChanged(object sender, EventArgs e)
    { 
        var searchText = ((TextEdit)sender).Text;
        protocols.FilterString =$"Contains([FullAddress], '{searchText}') " +
            $"or Contains([FireEscapeObject], '{searchText}') " +
            $"or Contains([Customer], '{searchText}') " +
            $"or Contains([Details], '{searchText}')";
    }

    void CollectionViewChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (ProtocolMainViewModel != null)
            ProtocolMainViewModel.IsEmptyList = protocols.VisibleItemCount == 0 && !ProtocolMainViewModel.IsRefreshing;
    }

    void tapPressed(object sender, DevExpress.Maui.Core.DXTapEventArgs e)
    {
        tapStopwatch.Reset();
        tapStopwatch.Start();
    }

    void tapReleased(object sender, DevExpress.Maui.Core.DXTapEventArgs e)
    {
        tapStopwatch.Stop();
        if (tapStopwatch.ElapsedMilliseconds > 4000)
            ProtocolMainViewModel?.OpenUserAccountMainPageCommand.Execute(null);
    }
}