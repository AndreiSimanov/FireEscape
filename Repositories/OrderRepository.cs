using FireEscape.DBContext;
using FireEscape.Factories;
using SQLite;

namespace FireEscape.Repositories
{
    public class OrderRepository(SqliteContext context, OrderFactory orderFactory) : IOrderRepository
    {
        readonly AsyncLazy<SQLiteAsyncConnection> connection = context.Connection;

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
        }

        public async Task<Order[]> GetOrdersAsync()
        {
            var query = (await connection).Table<Order>().OrderByDescending(item => item.Created);
            return await query.ToArrayAsync();
        }
    }
}
