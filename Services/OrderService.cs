namespace FireEscape.Services;

public class OrderService(IOrderRepository orderRepository, ISearchDataRepository searchDataRepository)
{
    public async Task<Order> CreateOrderAsync() => await orderRepository.CreateOrderAsync();

    public async Task SaveOrderAsync(Order order)
    {
        await orderRepository.SaveOrderAsync(order);
        await searchDataRepository.SetSearchDataAsync(order.Id);
    }

    public async Task DeleteOrderAsync(Order order) => await orderRepository.DeleteOrderAsync(order);

    public async Task<PagedResult<Order>> GetOrdersAsync(string searchText, PagingParameters pageParams) => await orderRepository.GetOrdersAsync(searchText, pageParams);
}
