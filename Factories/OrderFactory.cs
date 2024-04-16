using Microsoft.Extensions.Options;

namespace FireEscape.Factories
{
    public class OrderFactory(IOptions<NewOrderSettings> newOrderSettings)
    {
        readonly NewOrderSettings newOrderSettings = newOrderSettings.Value;
        public Order CreateDefaultOrder() => new Order()
        {
            Location = newOrderSettings.Location,
            Created = DateTime.Now,
            Updated = DateTime.Now
        };
    }
}
