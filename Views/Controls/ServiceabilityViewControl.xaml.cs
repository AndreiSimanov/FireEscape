using FireEscape.Converters;

namespace FireEscape.Views.Controls;

public partial class ServiceabilityViewControl : BaseServiceabilityViewControl
{
    public ServiceabilityViewControl()
    {
        InitializeComponent();
        grid.BindingContext = this;
    }

    public string? UnitOfMeasureSymbol { get; set; }

    protected override void UpdateValueConverter()
    {
        valueSpan.RemoveBinding(Span.TextProperty);
        valueSpan.SetBinding(
            Span.TextProperty,
            new Binding("ServiceabilityValue.Value", converter: ValueConverter));
        UnitOfMeasureSymbol = ValueConverter is UnitOfMeasureConverter unitOfMeasureConverter ? unitOfMeasureConverter.UnitOfMeasure.Symbol : string.Empty;
        OnPropertyChanged(nameof(UnitOfMeasureSymbol));
    }
}