using FireEscape.DBContext;
using FireEscape.Factories.Interfaces;

namespace FireEscape.Repositories;

public class StairsRepository(SqliteContext context, IStairsFactory factory)
    : BaseObjectRepository<Stairs, Protocol>(context, factory), IStairsRepository
{
    public async Task<Stairs> CopyAsync(Stairs stairs)
    {
        var newStairs = factory.CopyStairs(stairs);
        await SaveAsync(newStairs);
        return newStairs;
    }
}
