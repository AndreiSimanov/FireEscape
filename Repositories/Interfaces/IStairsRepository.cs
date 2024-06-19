namespace FireEscape.Repositories.Interfaces;
public interface IStairsRepository : IBaseObjectRepository<Stairs, Protocol>
{
    Task<Stairs> CopyAsync(Stairs stairs);
}