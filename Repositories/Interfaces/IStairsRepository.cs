namespace FireEscape.Repositories.Interfaces;
public interface IStairsRepository : IBaseObjectRepository<Stairs, Protocol>
{
    Task<Stairs> CopyAsync(Stairs stairs);
    IEnumerable<BaseStairsElement> GetAvailableStairsElements(Stairs stairs);
    BaseStairsElement CopyStairsElement(Stairs stairs, BaseStairsElement stairsElement);
}