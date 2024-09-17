using FireEscape.DBContext;
using FireEscape.Factories.Interfaces;
using System.Linq.Expressions;

namespace FireEscape.Repositories;

public class OrderRepository(SqliteContext context, IOrderFactory factory) : BaseObjectRepository<Order, BaseObject>(context, factory), IOrderRepository
{
    public override async Task DeleteAsync(Order order)
    {
        if (order.Id == 0)
            return;
        await (await connection).RunInTransactionAsync(connection =>
        {
            connection.Table<Protocol>().Where(protocol => protocol.OrderId == order.Id).Delete();
            connection.Table<Stairs>().Where(stairs => stairs.OrderId == order.Id).Delete();
            base.DeleteAsync(order).Wait();
        });

        var imageFileMask = $"{order.Id}_*.{ImageUtils.IMAGE_FILE_EXTENSION}";
        var dir = new DirectoryInfo(await ApplicationSettings.GetImagesFolderAsync());
        foreach (var file in dir.EnumerateFiles(imageFileMask))
            file.Delete();
    }

    public async Task<PagedResult<Order>> GetOrdersAsync(string searchText, PagingParameters pageParams)
    {
        var query = (await connection).Table<Order>();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            searchText = searchText.Trim().ToLowerInvariant();
            Expression<Func<Order, bool>> expr = order => order.SearchData.Contains(searchText);

            if (int.TryParse(searchText, out int orderId))
            {
                Expression<Func<Order, bool>> idExpr = order => order.Id == orderId;
                expr = CombineOr(expr, idExpr);
            }
            query = query.Where(expr);
        }
        query = AddSortAndPaging(query, pageParams);

        var orders = await query.ToArrayAsync();
        return PagedResult<Order>.Create(orders, orders.Length >= pageParams.Take);
    }

    static AsyncTableQuery<Order> AddSortAndPaging(AsyncTableQuery<Order> tableQuery, PagingParameters pageParams)
    {
        return tableQuery.OrderByDescending(item => item.Id).Skip(pageParams.Skip).Take(pageParams.Take);
    }

    static Expression<Func<T, bool>> CombineOr<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        return Expression.Lambda<Func<T, bool>>(Expression.Or(expr1.Body, expr2.Body), expr1.Parameters[0]);
    }
}