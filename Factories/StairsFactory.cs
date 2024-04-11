using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;

namespace FireEscape.Factories
{
    public class StairsFactory(IOptions<StairsSettings> stairsSettings)
    {
        readonly StairsSettings stairsSettings = stairsSettings.Value;

        public Stairs CreateDefaultStairs() => new Stairs()
        {
            StairsHeight = new ServiceabilityProperty<float?>() { Serviceability = stairsSettings.ServiceabilityTypes![0] },
            StairsWidth = new ServiceabilityProperty<int?>() { Serviceability = stairsSettings.ServiceabilityTypes![0] },
            StairsType = stairsSettings.StairsTypes![0],
            StairsMountType = stairsSettings.StairsMountTypes![0],
            StairsElements = new ObservableCollection<BaseStairsElement>(GetRequiredStairsElements())
        };

        private IEnumerable<BaseStairsElement> GetRequiredStairsElements()
        {
            if (stairsSettings.StairsElementSettings == null)
                yield break;
            foreach (var elementSetting in stairsSettings.StairsElementSettings.Where(item => item.Required))
            {
                var elementType = Type.GetType(elementSetting.Type);
                if (elementType == null)
                    continue;
                var stairsElement = Activator.CreateInstance(elementType) as BaseStairsElement;
                if (stairsElement == null)
                    continue;
                stairsElement.Order = elementSetting.Order;
                stairsElement.TestPointCount = elementSetting.TestPointCount;
                stairsElement.WithstandLoad = elementSetting.WithstandLoad;
                yield return stairsElement;
            }
        }
    }
}
