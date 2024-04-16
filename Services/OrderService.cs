namespace FireEscape.Services
{
    public class OrderService(IOrderRepository orderRepository)
    {
        public async Task<Order> CreateOrderAsync() => await orderRepository.CreateOrderAsync();

        public async Task SaveOrderAsync(Order order) => await orderRepository.SaveOrderAsync(order);

        public async Task DeleteOrderAsync(Order order) => await orderRepository.DeleteOrderAsync(order);

        public async Task<Order[]> GetOrdersAsync() => await orderRepository.GetOrdersAsync();
    }
}
