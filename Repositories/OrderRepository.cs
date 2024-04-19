using DevExpress.Maui.Core.Internal;
using FireEscape.DBContext;
using FireEscape.Factories;
using Microsoft.Extensions.Options;
using SQLite;
using System.Linq.Expressions;

namespace FireEscape.Repositories;

public class OrderRepository(SqliteContext context, IOptions<ApplicationSettings> applicationSettings, OrderFactory orderFactory) : IOrderRepository
{
    readonly AsyncLazy<SQLiteAsyncConnection> connection = context.Connection;
    readonly ApplicationSettings applicationSettings = applicationSettings.Value;

    public async Task<Order> CreateOrderAsync()
    {
        var order = orderFactory.CreateDefaultOrder();
        await SaveOrderAsync(order);
        return order;
    }

    public async Task<Order> SaveOrderAsync(Order order)
    {
        if (order.Id != 0)
        {
            order.Updated = DateTime.Now;
            await (await connection).UpdateAsync(order);
        }
        else
            await (await connection).InsertAsync(order);
        return order;
    }

    public async Task DeleteOrderAsync(Order order)
    {
        if (order.Id == 0)
            return;
        await (await connection).RunInTransactionAsync(connection =>
        {
            connection.Table<Protocol>().Where(protocol => protocol.OrderId == order.Id).Delete();
            connection.Delete(order);
        });

        var imageFileMask = $"{order.Id}_*.{ImageUtils.IMAGE_FILE_EXTENSION}";
        var dir = new DirectoryInfo(applicationSettings.ContentFolder);
        dir.EnumerateFiles(imageFileMask).ForEach(file => file.Delete());
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
