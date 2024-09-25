using FireEscape.Factories.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace FireEscape.Factories;

public class StairsFactory(IOptions<StairsSettings> stairsSettings) : IStairsFactory
{
    readonly StairsSettings stairsSettings = stairsSettings.Value;

    public Stairs CreateDefault(Protocol? protocol) => new()
    {
        OrderId = (protocol == null) ? 0 : protocol.OrderId,
        Created = DateTime.Now,
        Updated = DateTime.Now,
        StairsType = StairsTypeEnum.P1_1,
        StairsMountType = StairsMountTypeEnum.BuildingMounted,
        WeldSeamServiceability = stairsSettings.WeldSeamServiceability,
        ProtectiveServiceability = stairsSettings.ProtectiveServiceability,
        StairsElements = new ObservableCollection<BaseStairsElement>(GetDefaultStairsElements())
    };

    public Stairs CopyStairs(Stairs stairs)
    {
        if (AppUtils.TryDeserialize<Stairs>(JsonSerializer.Serialize(stairs), out var copy))
        {
            copy!.Id = 0;
            copy.Created = DateTime.Now;
            copy.Updated = DateTime.Now;
            return copy;
        }
        throw new Exception(AppResources.CopyStairsError);
    }

    public IEnumerable<BaseStairsElement> GetAvailableStairsElements(Stairs stairs)
    {
        if (stairsSettings.StairsElementSettings == null)
            yield break;
        foreach (var elementSettings in stairsSettings.StairsElementSettings.Where(item => item.BaseStairsType == stairs.BaseStairsType))
        {
            var elementType = Type.GetType(elementSettings.TypeName);
            if (elementType == null)
                continue;
            var stairsElements = stairs.StairsElements.Where(item => item.StairsElementType == elementType).ToList();
            var elementsCount = stairsElements.Count;
            if (elementsCount >= elementSettings.MaxCount)
                continue;
            var elementNumber = stairsElements.Count != 0 ? stairsElements.Max(element => element.ElementNumber) + 1 : 1;
            var stairsElement = CreateElement(elementType, elementNumber, elementSettings);
            if (stairsElement == null)
                continue;
            yield return stairsElement;
        }
    }

    public BaseStairsElement CopyStairsElement(Stairs stairs, BaseStairsElement stairsElement)
    {
        var availableStairsElement = GetAvailableStairsElements(stairs).FirstOrDefault(item => item.StairsElementType == stairsElement.StairsElementType);
        if (availableStairsElement != null && AppUtils.TryDeserialize<BaseStairsElement>(JsonSerializer.Serialize(stairsElement), out var copy))
        {
            copy!.ElementNumber = availableStairsElement!.ElementNumber;
            return copy;
        }
        throw new Exception(AppResources.CopyStairsElementError);
    }

    static BaseStairsElement? CreateElement(Type type, int elementNumber, StairsElementSettings elementSettings)
    {
        if (Activator.CreateInstance(type) is BaseStairsElement stairsElement)
        {
            stairsElement.WithstandLoad = elementSettings.WithstandLoad;
            stairsElement.ElementNumber = elementNumber;
            if (stairsElement is BaseSupportBeamsElement element)
                element.SupportBeamsCount = elementSettings.SupportBeamsCount;
            return stairsElement;
        }
        return null;
    }

    IEnumerable<BaseStairsElement> GetDefaultStairsElements()
    {
        if (stairsSettings.StairsElementSettings == null)
            yield break;
        foreach (var elementSetting in stairsSettings.StairsElementSettings.Where(item => item.AddToStairsByDefault))
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
