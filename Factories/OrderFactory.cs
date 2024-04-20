using FireEscape.Factories.Interfaces;
using Microsoft.Extensions.Options;

namespace FireEscape.Factories;

public class OrderFactory(IOptions<NewOrderSettings> newOrderSettings) : IOrderFactory
{
    readonly NewOrderSettings newOrderSettings = newOrderSettings.Value;
    public Order CreateDefault(BaseObject? parent) => new()
    {
        Location = newOrderSettings.Location,
        Created = DateTime.Now,
        Updated = DateTime.Now
    };
}
