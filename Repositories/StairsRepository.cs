using FireEscape.DBContext;
using FireEscape.Factories.Interfaces;
using SQLiteNetExtensionsAsync.Extensions;

namespace FireEscape.Repositories;

public class StairsRepository(SqliteContext context, IStairsFactory factory)
    : BaseObjectRepository<Stairs, Protocol>(context, factory), IStairsRepository
{
    public async Task<Dictionary<int, Stairs>> GetStairsesAsync(int orderId)
    {
        var stairses = await (await connection)
            .GetAllWithChildrenAsync<Stairs>(stairs => stairs.OrderId == orderId);
        return stairses.ToDictionary(stairs => stairs.ProtocolId);
    }
}