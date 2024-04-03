namespace FireEscape.Views.Controls;

public partial class ServiceabilityControl : ContentView
{
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(decimal?), typeof(ServiceabilityControl), defaultBindingMode: BindingMode.TwoWay);
    public decimal? Value
    {
        get => (decimal?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly BindableProperty ServiceabilityProperty = BindableProperty.Create(nameof(Serviceability), typeof(Serviceability), typeof(ServiceabilityControl), defaultBindingMode: BindingMode.TwoWay);
    public Serviceability Serviceability
    {
        get => (Serviceability)GetValue(ServiceabilityProperty);
        set => SetValue(ServiceabilityProperty, value);
    }

    public static readonly BindableProperty ServiceabilityTypesProperty = BindableProperty.Create(nameof(ServiceabilityTypes), typeof(Serviceability[]), typeof(ServiceabilityControl));
    public Serviceability[] ServiceabilityTypes
    {
        get => (Serviceability[])GetValue(ServiceabilityTypesProperty);
        set => SetValue(ServiceabilityTypesProperty, value);
    }

    public static readonly BindableProperty RejectExplanationTextProperty = BindableProperty.Create(nameof(RejectExplanationText), typeof(string), typeof(ServiceabilityControl), defaultBindingMode: BindingMode.TwoWay );
    public string RejectExplanationText
    {
        get => (string)GetValue(RejectExplanationTextProperty);
        set => SetValue(RejectExplanationTextProperty, value);
    }

    public string LabelText { get=> numEdit.LabelText; set => numEdit.LabelText = value; }
    public string PlaceholderText { get => numEdit.PlaceholderText; set => numEdit.PlaceholderText = value; }
    public decimal MinValue { get => numEdit.MinValue; set => numEdit.MinValue = value; }
    public decimal MaxValue { get => numEdit.MaxValue; set => numEdit.MaxValue = value; }
    public int MaxDecimalDigitCount { get => numEdit.MaxDecimalDigitCount; set => numEdit.MaxDecimalDigitCount = value; }

    public ServiceabilityControl()
    {
        InitializeComponent();
    }
}