namespace FireEscape.Factories.Interfaces;

public interface IStairsFactory : IBaseObjectFactory<Stairs, Protocol>
{
    IEnumerable<BaseStairsElement> GetAvailableStairsElements(Stairs stairs);
    BaseStairsElement CopyStairsElement(Stairs stairs, BaseStairsElement stairsElement);
    Stairs CopyStairs(Stairs stairs);
}