using FireEscape.DBContext;
using FireEscape.Factories.Interfaces;

namespace FireEscape.Repositories;

public class StairsRepository(SqliteContext context, IStairsFactory factory)
    : BaseObjectRepository<Stairs, Protocol>(context, factory), IStairsRepository
{
    public Task<Stairs> CopyAsync(Stairs stairs) => SaveAsync(factory.CopyStairs(stairs));
    public IEnumerable<BaseStairsElement> GetAvailableStairsElements(Stairs stairs) => factory.GetAvailableStairsElements(stairs);
    public BaseStairsElement CopyStairsElement(Stairs stairs, BaseStairsElement stairsElement) => 
        factory.CopyStairsElement(stairs, stairsElement);
}
