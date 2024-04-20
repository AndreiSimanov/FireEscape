namespace FireEscape.Factories.Interfaces;

public interface IStairsFactory : IBaseObjectFactory<Stairs, Protocol>
{
    IEnumerable<BaseStairsElement> GetAvailableStairsElements(Stairs stairs);
}