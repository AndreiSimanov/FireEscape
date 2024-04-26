using FireEscape.DBContext;
using FireEscape.Factories.Interfaces;

namespace FireEscape.Repositories;

public class StairsRepository(SqliteContext context, IStairsFactory factory)
    : BaseObjectRepository<Stairs, Protocol>(context, factory), IStairsRepository
{
    public async Task<Dictionary<int, Stairs>> GetStairsesAsync(int orderId)
    {
        var stairses = await  (await connection)
            .Table<Stairs>()
            .Where(stairs => stairs.OrderId == orderId).ToArrayAsync();
        return stairses.ToDictionary(stairs => stairs.ProtocolId);
    }
}