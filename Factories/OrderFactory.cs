using FireEscape.Factories.Interfaces;
using Microsoft.Extensions.Options;

namespace FireEscape.Factories;

public class OrderFactory(IOptions<OrderSettings> OrderSettings) : IOrderFactory
{
    readonly OrderSettings OrderSettings = OrderSettings.Value;
    public Order CreateDefault(BaseObject? parent) => new()
    {
        Location = OrderSettings.Location,
        Created = DateTime.Now,
        Updated = DateTime.Now
    };
}
