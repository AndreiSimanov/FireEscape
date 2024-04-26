namespace FireEscape.Services;

public class OrderService(IOrderRepository orderRepository, ISearchDataRepository searchDataRepository)
{
    public async Task<Order> CreateAsync() => await orderRepository.CreateAsync(null);

    public async Task SaveAsync(Order order)
    {
        await orderRepository.SaveAsync(order);
        await searchDataRepository.SetSearchDataAsync(order.Id);
    }

    public async Task DeleteAsync(Order order) => await orderRepository.DeleteAsync(order);

    public async Task<PagedResult<Order>> GetOrdersAsync(string searchText, PagingParameters pageParams) => await orderRepository.GetOrdersAsync(searchText, pageParams);
}
