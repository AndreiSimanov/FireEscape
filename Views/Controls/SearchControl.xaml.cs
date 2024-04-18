using System.Windows.Input;

namespace FireEscape.Views.Controls;

public partial class SearchControl : ContentView
{

    public static readonly BindableProperty SearchProperty = BindableProperty.Create(nameof(Search), typeof(string), typeof(SearchControl), defaultBindingMode: BindingMode.TwoWay);
    public string Search
    {
        get => (string)GetValue(SearchProperty);
        set => SetValue(SearchProperty, value);
    }

    public static readonly BindableProperty SearchCommandProperty = BindableProperty.Create(nameof(SearchCommand), typeof(ICommand), typeof(SearchControl));
    public ICommand SearchCommand
    {
        get => (ICommand)GetValue(SearchCommandProperty);
        set => SetValue(SearchCommandProperty, value);
    }

    public int StoppedTypingTimeThreshold { get => stoppedTyping.StoppedTypingTimeThreshold; set => stoppedTyping.StoppedTypingTimeThreshold = value; }
    public string Placeholder { get => searchEntry.Placeholder; set => searchEntry.Placeholder = value; }
  
    public SearchControl()
    {
        InitializeComponent();
    }
}