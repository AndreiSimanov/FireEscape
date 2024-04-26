
namespace FireEscape.Services;



public class StairsService(IStairsRepository stairsRepository)
{
    public async Task SaveStairsAsync(Stairs stairs) => await stairsRepository.SaveAsync(stairs);
}
