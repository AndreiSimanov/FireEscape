using FireEscape.Factories.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;

namespace FireEscape.Factories;

public class StairsFactory(IOptions<StairsSettings> stairsSettings) : IStairsFactory
{
    readonly StairsSettings stairsSettings = stairsSettings.Value;

    public Stairs CreateDefault(Protocol? protocol) => new()
    {
        OrderId = (protocol == null) ? 0 : protocol.OrderId,
        Created = DateTime.Now,
        Updated = DateTime.Now,
        StairsHeight = new ServiceabilityProperty<float?>() { Serviceability = stairsSettings.ServiceabilityTypes![0] },
        StairsWidth = new ServiceabilityProperty<int?>() { Serviceability = stairsSettings.ServiceabilityTypes![0] },
        StairsType = stairsSettings.StairsTypes![0],
        StairsMountType = stairsSettings.StairsMountTypes![0],
        WeldSeamServiceability = stairsSettings.WeldSeamServiceability,
        ProtectiveServiceability = stairsSettings.ProtectiveServiceability,
        StairsElements = new ObservableCollection<BaseStairsElement>(GetDefaultStairsElements())
    };

    public IEnumerable<BaseStairsElement> GetAvailableStairsElements(Stairs stairs)
    {
        if (stairsSettings.StairsElementSettings == null)
            yield break;
        foreach (var elementSettings in stairsSettings.StairsElementSettings.Where(item => item.BaseStairsType == stairs.StairsType.BaseStairsType))
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

    BaseStairsElement? CreateElement(Type type)
    {
        if (stairsSettings.StairsElementSettings == null)
            return null;
        var elementSettings = stairsSettings.StairsElementSettings.Where(item => item.Type == type.FullName).FirstOrDefault();
        if (elementSettings == null)
            return null;
        return CreateElement(type, elementSettings);
    }

    BaseStairsElement? CreateElement(Type type, StairsElementSettings elementSettings)
    {
        var stairsElement = Activator.CreateInstance(type) as BaseStairsElement;
        if (stairsElement != null)
        {
            stairsElement.PrintOrder = elementSettings.PrintOrder;
            stairsElement.WithstandLoad = elementSettings.WithstandLoad;
            if (elementSettings.MaxCount > 1)
                stairsElement.ElementNumber = "1";
            stairsElement.Deformation = new ServiceabilityProperty<float?>() { Serviceability = stairsSettings.ServiceabilityTypes![0] };
            return stairsElement;
        }
        return null;
    }

    IEnumerable<BaseStairsElement> GetDefaultStairsElements()
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
