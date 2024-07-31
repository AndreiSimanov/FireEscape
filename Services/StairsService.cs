namespace FireEscape.Services;

public class StairsService(IStairsRepository stairsRepository)
{
    public Task SaveAsync(Stairs stairs) => stairsRepository.SaveAsync(stairs);
}
