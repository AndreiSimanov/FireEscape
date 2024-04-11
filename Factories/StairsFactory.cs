﻿using Microsoft.Extensions.Options;
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

        public IEnumerable<BaseStairsElement> GetAvailableStairsElements(Stairs stairs)
        {
            if (stairsSettings.StairsElementSettings == null)
                yield break;
            foreach (var elementSettings in stairsSettings.StairsElementSettings.Where(item => (int)item.StairsType == (int)stairs.StairsType.Type))
            {
                var elementType = Type.GetType(elementSettings.Type);
                if (elementType == null)
                    continue;
                var elementsCount = stairs.StairsElements.Where(item => item.GetType() == elementType).Count();
                if (elementsCount >= elementSettings.MaxCount)
                    continue;
                var stairsElement = CreateElement(elementType, elementSettings);
                if (stairsElement == null)
                    continue;
                yield return stairsElement;
            }
        }

        static BaseStairsElement? CreateElement(Type type, StairsElementSettings elementSettings)
        {
            var stairsElement = Activator.CreateInstance(type) as BaseStairsElement;
            if (stairsElement != null)
            {
                stairsElement.Order = elementSettings.Order;
                stairsElement.TestPointCount = elementSettings.TestPointCount;
                stairsElement.WithstandLoad = elementSettings.WithstandLoad;
                if (elementSettings.MaxCount > 1)
                    stairsElement.ElementNumber = "1";
            }
            return stairsElement;
        }

        IEnumerable<BaseStairsElement> GetRequiredStairsElements()
        {
            if (stairsSettings.StairsElementSettings == null)
                yield break;
            foreach (var elementSetting in stairsSettings.StairsElementSettings.Where(item => item.Required))
            {
                var elementType = Type.GetType(elementSetting.Type);
                if (elementType == null)
                    continue;
                var stairsElement = CreateElement(elementType, elementSetting);
                if (stairsElement == null)
                    continue;
                yield return stairsElement;
            }
        }
    }
}
