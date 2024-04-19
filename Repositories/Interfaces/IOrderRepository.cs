namespace FireEscape.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<Order> CreateOrderAsync();
    Task DeleteOrderAsync(Order order);
    Task<PagedResult<Order>> GetOrdersAsync(string searchText, PagingParameters pageParams);
    Task<Order> SaveOrderAsync(Order order);
}
