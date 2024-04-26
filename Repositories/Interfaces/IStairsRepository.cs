namespace FireEscape.Repositories.Interfaces;
public interface IStairsRepository : IBaseObjectRepository<Stairs, Protocol>
{
    Task<Dictionary<int, Stairs>> GetStairsesAsync(int orderId);
}