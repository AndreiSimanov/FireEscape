namespace FireEscape.Views.Controls;

public abstract class BaseServiceabilityViewControl<T> : ContentView where T : struct
{
    public static readonly BindableProperty LabelProperty = BindableProperty.Create(nameof(Label), typeof(string), typeof(BaseServiceabilityViewControl<T>), defaultBindingMode: BindingMode.OneWay);
    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly BindableProperty ServiceabilityValueProperty = BindableProperty.Create(nameof(ServiceabilityValue), typeof(ServiceabilityProperty<T>), typeof(BaseServiceabilityViewControl<T>), defaultBindingMode: BindingMode.OneWay);
    public ServiceabilityProperty<T> ServiceabilityValue
    {
        get => (ServiceabilityProperty<T>)GetValue(ServiceabilityValueProperty);
        set => SetValue(ServiceabilityValueProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(BaseServiceabilityViewControl<T>), defaultBindingMode: BindingMode.OneWay);
    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    IValueConverter? valueConverter;
    public IValueConverter? ValueConverter
    {
        get => valueConverter;
        set
        {
            if (valueConverter != value)
            {
                valueConverter = value;
                UpdateValueConverter();
            }
        }
    }
    protected abstract void UpdateValueConverter();
}