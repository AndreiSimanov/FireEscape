namespace FireEscape.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync();
        Task DeleteOrderAsync(Order order);
        Task<Order[]> GetOrdersAsync();
        Task<Order> SaveOrderAsync(Order order);
    }
}
