namespace FireEscape.Services;

public class StairsService(IStairsRepository stairsRepository)
{
    public async Task SaveAsync(Stairs stairs) => await stairsRepository.SaveAsync(stairs);
}
