namespace FireEscape.Views;

public partial class MainPage : ContentPage
{
    double swipeOffSet = 0;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private MainViewModel? MainViewModel
    {
        get
        {
            return BindingContext as MainViewModel;
        }
    }

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        if (MainViewModel != null)
        {
            MainViewModel.GetProtocolsCommand.Execute(null);
            if (MainViewModel.SelectedProtocol != null)
            {
                var index = MainViewModel.Protocols.IndexOf(MainViewModel.SelectedProtocol);
                protocols.ScrollTo(index);
                MainViewModel.SelectedProtocol = null;
            }
        }
    }

    private void OnSwipeChanging(object sender, SwipeChangingEventArgs args)
    {
        swipeOffSet = args.Offset;
    }

    private void OnSwipeEnded(object sender, SwipeEndedEventArgs args) // Reduce Swipeview Sensitivity .Net Maui https://stackoverflow.com/questions/72635530/reduce-swipeview-sensitivity-net-maui
    {
        if (!args.IsOpen && swipeOffSet < 5 && swipeOffSet > -5 && MainViewModel != null)
        {
            MainViewModel.GoToDetailsCommand.Execute(protocols.SelectedItem);
        }
    }
}
