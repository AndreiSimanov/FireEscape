﻿namespace FireEscape.Services;

public class OrderService(IOrderRepository orderRepository, UserAccountService userAccountService, ISearchDataRepository searchDataRepository)
{
    public async Task<Order> CreateAsync()
    {
        var order = await orderRepository.CreateAsync(null);
        var userAccount = await userAccountService.GetCurrentUserAccountAsync();
        if (userAccount != null && !string.IsNullOrWhiteSpace(userAccount.Signature))
        {
            order.PrimaryExecutorSign = userAccount.Signature;
            await orderRepository.SaveAsync(order);
        }
        return order;
    }

    public async Task SaveAsync(Order order)
    {
        await orderRepository.SaveAsync(order);
        await searchDataRepository.SetSearchDataAsync(order.Id);
    }

    public Task DeleteAsync(Order order) => orderRepository.DeleteAsync(order);

    public Task<PagedResult<Order>> GetOrdersAsync(string searchText, PagingParameters pageParams) => orderRepository.GetOrdersAsync(searchText, pageParams);
}
