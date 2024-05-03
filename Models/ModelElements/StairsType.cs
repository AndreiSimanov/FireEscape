using Newtonsoft.Json;

namespace FireEscape.Models;

public enum BaseStairsTypeEnum
{
    P1 = 1,
    P2 = 2
}

public enum StairsTypeEnum
{
    P1_1 = 1,
    P1_2 = 2,
    P2 = 3
}

public readonly record struct StairsType(string Name, StairsTypeEnum Type)
{
    [JsonIgnore]
    public BaseStairsTypeEnum BaseStairsType => Type == StairsTypeEnum.P2 ?  BaseStairsTypeEnum.P2 : BaseStairsTypeEnum.P1;
    public override string ToString() => Name;
}
