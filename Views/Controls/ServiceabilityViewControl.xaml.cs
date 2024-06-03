using FireEscape.Converters;

namespace FireEscape.Views.Controls;

public partial class ServiceabilityViewControl : BaseServiceabilityViewControl<float>
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

        var unitOfMeasureConverter = ValueConverter as UnitOfMeasureConverter;

        UnitOfMeasureSymbol = unitOfMeasureConverter == null ? string.Empty : unitOfMeasureConverter.UnitOfMeasure.Symbol;
        OnPropertyChanged(nameof(UnitOfMeasureSymbol));
    }
}