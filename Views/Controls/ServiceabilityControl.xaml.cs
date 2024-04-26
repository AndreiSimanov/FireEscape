namespace FireEscape.Views.Controls;

public partial class ServiceabilityControl : ContentView
{
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(decimal?), typeof(ServiceabilityControl), defaultBindingMode: BindingMode.TwoWay);
    public decimal? Value
    {
        get => (decimal?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly BindableProperty ServiceabilityProperty = BindableProperty.Create(nameof(Serviceability), typeof(ServiceabilityType), typeof(ServiceabilityControl), defaultBindingMode: BindingMode.TwoWay);
    public ServiceabilityType Serviceability
    {
        get => (ServiceabilityType)GetValue(ServiceabilityProperty);
        set => SetValue(ServiceabilityProperty, value);
    }

    public static readonly BindableProperty ServiceabilityTypesProperty = BindableProperty.Create(nameof(ServiceabilityTypes), typeof(ServiceabilityType[]), typeof(ServiceabilityControl));
    public ServiceabilityType[] ServiceabilityTypes
    {
        get => (ServiceabilityType[])GetValue(ServiceabilityTypesProperty);
        set => SetValue(ServiceabilityTypesProperty, value);
    }

    public static readonly BindableProperty RejectExplanationTextProperty = BindableProperty.Create(nameof(RejectExplanationText), typeof(string), typeof(ServiceabilityControl), defaultBindingMode: BindingMode.TwoWay );
    public string RejectExplanationText
    {
        get => (string)GetValue(RejectExplanationTextProperty);
        set => SetValue(RejectExplanationTextProperty, value);
    }

    public static readonly BindableProperty LabelTextProperty = BindableProperty.Create(nameof(LabelText), typeof(string), typeof(ServiceabilityControl), defaultBindingMode: BindingMode.TwoWay);
    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(ServiceabilityControl), defaultBindingMode: BindingMode.TwoWay);
    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public decimal MinValue { get => numEdit.MinValue; set => numEdit.MinValue = value; }
    public decimal MaxValue { get => numEdit.MaxValue; set => numEdit.MaxValue = value; }
    public int MaxDecimalDigitCount { get => numEdit.MaxDecimalDigitCount; set => numEdit.MaxDecimalDigitCount = value; }

    public ServiceabilityControl() => InitializeComponent();
}