using FireEscape.ViewModels;

namespace FireEscape.View;

public partial class MainPage : ContentPage
{
    public MainPage(ProtocolsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
