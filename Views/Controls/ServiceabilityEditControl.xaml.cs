using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace FireEscape.Views.Controls;

public partial class ServiceabilityEditControl : ContentView
{
    public static readonly BindableProperty ServiceabilityValueProperty = BindableProperty.Create(nameof(ServiceabilityValue), typeof(ServiceabilityProperty<float>), typeof(ServiceabilityEditControl), defaultBindingMode: BindingMode.TwoWay);
    public ServiceabilityProperty<float> ServiceabilityValue
    {
        get => (ServiceabilityProperty<float>)GetValue(ServiceabilityValueProperty);
        set => SetValue(ServiceabilityValueProperty, value);
    }

    public static readonly BindableProperty ServiceabilityTypesProperty = BindableProperty.Create(nameof(ServiceabilityTypes), typeof(string[]), typeof(ServiceabilityEditControl));
    public string[] ServiceabilityTypes
    {
        get => (string[])GetValue(ServiceabilityTypesProperty);
        set => SetValue(ServiceabilityTypesProperty, value);
    }

    public static readonly BindableProperty LabelTextProperty = BindableProperty.Create(nameof(LabelText), typeof(string), typeof(ServiceabilityEditControl), defaultBindingMode: BindingMode.OneWay);
    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(ServiceabilityEditControl), defaultBindingMode: BindingMode.OneWay);
    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly BindableProperty ValueChangedCommandProperty = BindableProperty.Create(nameof(ValueChangedCommand), typeof(ICommand), typeof(ServiceabilityEditControl));
    public ICommand ValueChangedCommand
    {
        get => (ICommand)GetValue(ValueChangedCommandProperty);
        set => SetValue(ValueChangedCommandProperty, value);
    }

    public event EventHandler<FocusEventArgs> EditorFocused
    {
        add { numEdit.EditorFocused += value; }
        remove { numEdit.EditorFocused -= value; }
    }

    public IValueConverter? ValueConverter
    {
        get => numEdit.ValueConverter;
        set => numEdit.ValueConverter = value;
    }

    public string RejectExplanationLabel => $"{AppResources.RejectExplanation} ({LabelText?.ToLower()})";

    public ServiceabilityEditControl()
    {
        InitializeComponent();
        grid.BindingContext = this;
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(LabelText)) 
        {
            OnPropertyChanged(nameof(RejectExplanationLabel));
        }
    }
}