using System.ComponentModel;
using System.Resources;

namespace FireEscape.Common;

public class LocalizedDescriptionAttribute(string resourceKey, Type resourceType) : DescriptionAttribute
{
    readonly ResourceManager resourceManager = new ResourceManager(resourceType);
    readonly string resourceKey = resourceKey;

    public override string Description
    {
        get
        {
            var description = resourceManager.GetString(resourceKey);
            return string.IsNullOrWhiteSpace(description) ? string.Format("[[{0}]]", resourceKey) : description;
        }
    }
}
