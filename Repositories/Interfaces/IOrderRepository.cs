namespace FireEscape.Repositories.Interfaces;

public interface IOrderRepository : IBaseObjectRepository<Order, BaseObject>
{
    Task<PagedResult<Order>> GetOrdersAsync(string searchText, PagingParameters pageParams);
}