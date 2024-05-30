using DevExpress.Maui.Editors;
using FireEscape.Views.Converters;
using System.Windows.Input;

namespace FireEscape.Views.Controls;

public partial class ServiceabilityEditControl : ContentView
{
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(float), typeof(ServiceabilityEditControl), defaultBindingMode: BindingMode.TwoWay);
    public float Value
    {
        get => (float)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly BindableProperty ServiceabilityTypeProperty = BindableProperty.Create(nameof(ServiceabilityType), typeof(ServiceabilityTypeEnum), typeof(ServiceabilityEditControl), defaultBindingMode: BindingMode.TwoWay);
    public ServiceabilityTypeEnum ServiceabilityType
    {
        get => (ServiceabilityTypeEnum)GetValue(ServiceabilityTypeProperty);
        set => SetValue(ServiceabilityTypeProperty, value);
    }

    public static readonly BindableProperty ServiceabilityTypesProperty = BindableProperty.Create(nameof(ServiceabilityTypes), typeof(string[]), typeof(ServiceabilityEditControl));
    public string[] ServiceabilityTypes
    {
        get => (string[])GetValue(ServiceabilityTypesProperty);
        set => SetValue(ServiceabilityTypesProperty, value);
    }

    public static readonly BindableProperty RejectExplanationTextProperty = BindableProperty.Create(nameof(RejectExplanationText), typeof(string), typeof(ServiceabilityEditControl), defaultBindingMode: BindingMode.TwoWay);
    public string RejectExplanationText
    {
        get => (string)GetValue(RejectExplanationTextProperty);
        set => SetValue(RejectExplanationTextProperty, value);
    }

    public static readonly BindableProperty LabelTextProperty = BindableProperty.Create(nameof(LabelText), typeof(string), typeof(ServiceabilityEditControl), defaultBindingMode: BindingMode.OneWay);
    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty) + UnitOfMeasureSymbol;
        set => SetValue(LabelTextProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(ServiceabilityEditControl), defaultBindingMode: BindingMode.OneWay);
    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty) + UnitOfMeasureSymbol;
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
        add { numEdit.Focused += value; }
        remove { numEdit.Focused -= value; }
    }

    public string? UnitOfMeasureSymbol { get; set; }

    public decimal MinValue { get => numEdit.MinValue; set => numEdit.MinValue = value; }
    public decimal MaxValue { get => numEdit.MaxValue; set => numEdit.MaxValue = value; }
    public int MaxDecimalDigitCount { get => numEdit.MaxDecimalDigitCount; set => numEdit.MaxDecimalDigitCount = value; }

    IValueConverter? valueConverter;

    public IValueConverter? ValueConverter
    {
        get => valueConverter;
        set
        {
            if (valueConverter != value)
            {
                valueConverter = value;
                numEdit.RemoveBinding(NumericEdit.ValueProperty);
                numEdit.SetBinding(
                    NumericEdit.ValueProperty,
                    new Binding( nameof(Value),converter: valueConverter));

                var unitOfMeasureConverter = valueConverter as UnitOfMeasureConverter;
                if (unitOfMeasureConverter != null)
                {
                    MaxValue = unitOfMeasureConverter.UnitOfMeasure.MaxValue;
                    MaxDecimalDigitCount = unitOfMeasureConverter.UnitOfMeasure.MaxDecimalDigitCount;
                    UnitOfMeasureSymbol = $" ({unitOfMeasureConverter.UnitOfMeasure.Symbol})";
                }
                else
                    UnitOfMeasureSymbol = string.Empty;

                OnPropertyChanged(nameof(UnitOfMeasureSymbol));
                OnPropertyChanged(nameof(LabelText));
                OnPropertyChanged(nameof(PlaceholderText));
            }
        }
    }

    public ServiceabilityEditControl()
    {
        InitializeComponent();
        grid.BindingContext = this;
    }
}