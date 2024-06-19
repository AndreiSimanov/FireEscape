using FireEscape.DBContext;

namespace FireEscape.Repositories;

public class SearchDataRepository(SqliteContext context) : ISearchDataRepository
{
    readonly AsyncLazy<SQLiteAsyncConnection> connection = context.Connection;

    public async Task SetSearchDataAsync(int id)
    {
        if (id == 0)
            return;
        var conn = await connection;
        var protocolsSearchData = await conn.QueryScalarsAsync<string>("select distinct(Location || ' ' ||  Address || ' ' || FireEscapeObject) from Protocols where OrderId=?", id);
        var orderSearchData = await conn.QueryScalarsAsync<string>(
            "select Name || ' ' || " +
            "Location || ' ' ||  " +
            "Address || ' ' || " +
            "Customer || ' ' || " +
            "ExecutiveCompany || ' ' || " +
            "FireEscapeObject from Orders where Id=?", id);
        protocolsSearchData.AddRange(orderSearchData);
        var words = new HashSet<string>();
        protocolsSearchData.Where(item => !string.IsNullOrWhiteSpace(item)).
            SelectMany(token => token.Split(' ')).
            Where(token => !string.IsNullOrWhiteSpace(token)).
            ToList().
            ForEach(word => words.Add(word.Trim()));
        var searchData = string.Join(" ", words).ToLowerInvariant();
        await conn.ExecuteAsync("update Orders set SearchData=? where Id=?", [searchData, id]);
    }
}