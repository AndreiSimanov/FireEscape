using DevExpress.Maui.Editors;
using FireEscape.Views.Converters;
using System.Windows.Input;

namespace FireEscape.Views.Controls;

public partial class UnitOfMeasureEditControl : ContentView
{
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(float), typeof(UnitOfMeasureEditControl), defaultBindingMode: BindingMode.TwoWay);
    public float Value
    {
        get => (float)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly BindableProperty LabelTextProperty = BindableProperty.Create(nameof(LabelText), typeof(string), typeof(UnitOfMeasureEditControl), defaultBindingMode: BindingMode.OneWay);
    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty) + UnitOfMeasureSymbol;
        set => SetValue(LabelTextProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(UnitOfMeasureEditControl), defaultBindingMode: BindingMode.OneWay);
    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty) + UnitOfMeasureSymbol;
        set => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly BindableProperty ValueChangedCommandProperty = BindableProperty.Create(nameof(ValueChangedCommand), typeof(ICommand), typeof(UnitOfMeasureEditControl));
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

    public string? UnitOfMeasureSymbol { get; private set; }

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
                    new Binding(
                        nameof(Value),
                        converter: valueConverter));

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

    public UnitOfMeasureEditControl()
    {
        InitializeComponent();
        numEdit.BindingContext = this;
    }
}