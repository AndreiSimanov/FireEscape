namespace FireEscape.Services.Interfaces;

public interface IOrderService
{
    Task<Order> CreateAsync();
    Task DeleteAsync(Order order);
    Task<PagedResult<Order>> GetOrdersAsync(string searchText, PagingParameters pageParams);
    Task SaveAsync(Order order);
}