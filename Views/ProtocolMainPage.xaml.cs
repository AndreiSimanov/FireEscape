using DevExpress.Maui.Editors;
using System.Diagnostics;

namespace FireEscape.Views;

public partial class ProtocolMainPage : ContentPage
{
    double swipeOffSet = 0;
    Stopwatch tapStopwatch = new();

    public ProtocolMainPage(ProtocolMainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private ProtocolMainViewModel? ProtocolMainViewModel => BindingContext as ProtocolMainViewModel;
    
    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        ProtocolMainViewModel?.GetProtocolsCommand.Execute(null);
    }


    private void CreateProtocol(object sender, EventArgs e)
    {
        ProtocolMainViewModel?.AddProtocolCommand.Execute(null);
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

    private void CollectionViewChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (ProtocolMainViewModel != null)
            ProtocolMainViewModel.IsEmptyList = protocols.VisibleItemCount == 0;
    }

    private void tapPressed(object sender, DevExpress.Maui.Core.DXTapEventArgs e)
    {
        tapStopwatch.Reset();
        tapStopwatch.Start();
    }

    private void tapReleased(object sender, DevExpress.Maui.Core.DXTapEventArgs e)
    {
        tapStopwatch.Stop();
        if (tapStopwatch.ElapsedMilliseconds > 4000)
            ProtocolMainViewModel?.OpenUserAccountMainPageCommand.Execute(null);
    }
}