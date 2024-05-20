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
        StairsHeight = new ServiceabilityProperty<float>() { ServiceabilityType = ServiceabilityTypeEnum.Auto },
        StairsWidth = new ServiceabilityProperty<float>() { ServiceabilityType = ServiceabilityTypeEnum.Auto },
        StairsType = StairsTypeEnum.P1_1,
        StairsMountType =  StairsMountTypeEnum.BuildingMounted,
        WeldSeamServiceability = stairsSettings.WeldSeamServiceability,
        ProtectiveServiceability = stairsSettings.ProtectiveServiceability,
        StairsElements = new ObservableCollection<BaseStairsElement>(GetDefaultStairsElements())
    };

    public IEnumerable<BaseStairsElement> GetAvailableStairsElements(Stairs stairs)
    {
        if (stairsSettings.StairsElementSettings == null)
            yield break;
        foreach (var elementSettings in stairsSettings.StairsElementSettings.Where(item => item.BaseStairsType == stairs.BaseStairsType))
        {
            var elementType = Type.GetType(elementSettings.TypeName);
            if (elementType == null)
                continue;
            var stairsElements = stairs.StairsElements.Where(item => item.GetType() == elementType).ToList();
            var elementsCount = stairsElements.Count();
            if (elementsCount >= elementSettings.MaxCount)
                continue;
            var elementNumber = stairsElements.Any() ? stairsElements.Max(element => element.ElementNumber) + 1 : 1;
            var stairsElement = CreateElement(elementType, elementNumber, elementSettings);
            if (stairsElement == null)
                continue;
            yield return stairsElement;
        }
    }
    BaseStairsElement? CreateElement(Type type, int elementNumber, StairsElementSettings elementSettings)
    {
        var stairsElement = Activator.CreateInstance(type) as BaseStairsElement;
        if (stairsElement != null)
        {
            stairsElement.WithstandLoad = elementSettings.WithstandLoad;
            stairsElement.ElementNumber = elementNumber;
            if (stairsElement is BaseSupportBeamsElement)
                ((BaseSupportBeamsElement)stairsElement).SupportBeamsCount = elementSettings.SupportBeamsCount;
            stairsElement.Deformation = new ServiceabilityProperty<float>() { ServiceabilityType = ServiceabilityTypeEnum.Auto };
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
            var elementType = Type.GetType(elementSetting.TypeName);
            if (elementType == null)
                continue;
            var stairsElement = CreateElement(elementType, 1, elementSetting);
            if (stairsElement == null)
                continue;
            yield return stairsElement;
        }
    }
}
