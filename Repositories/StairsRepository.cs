using FireEscape.DBContext;
using FireEscape.Factories.Interfaces;

namespace FireEscape.Repositories;

public class StairsRepository(SqliteContext context, IStairsFactory factory)
    : BaseObjectRepository<Stairs, Protocol>(context, factory), IStairsRepository
{
    public Task<Stairs> CopyAsync(Stairs stairs) => SaveAsync(factory.CopyStairs(stairs));
}
