using Newtonsoft.Json;

namespace FireEscape.Models;

public readonly record struct StairsType(string Name, StairsTypeEnum Type)
{
    [JsonIgnore]
    public BaseStairsTypeEnum BaseStairsType => Type == StairsTypeEnum.P2 ?  BaseStairsTypeEnum.P2 : BaseStairsTypeEnum.P1;
    public override string ToString() => Name;
}
