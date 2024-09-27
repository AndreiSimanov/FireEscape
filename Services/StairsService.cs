using DevExpress.Data.Extensions;
using FluentValidation;
using FluentValidation.Results;

namespace FireEscape.Services;

public class StairsService(IStairsRepository stairsRepository, IValidator<Stairs> validator) : IStairsService
{
    public Task SaveAsync(Stairs stairs) => stairsRepository.SaveAsync(stairs);

    public ValidationResult Validate(Stairs stairs) => validator.Validate(stairs);

    public IEnumerable<BaseStairsElement> GetAvailableStairsElements(Stairs stairs)
    {
        var availableStairsElements = stairsRepository.GetAvailableStairsElements(stairs);

        if (stairs.BaseStairsType == BaseStairsTypeEnum.P2)
        {
            var platformIndex = stairs.StairsElements.FindIndex(element => element.StairsElementType == typeof(PlatformP2));
            var stairwayIndex = stairs.StairsElements.FindIndex(element => element.StairsElementType == typeof(StairwayP2));
            if ((stairwayIndex > platformIndex || stairwayIndex == -1) && platformIndex != -1)
                availableStairsElements = availableStairsElements.OrderBy(item => item.Name);
            else
                availableStairsElements = availableStairsElements.OrderByDescending(item => item.Name);
        }
        return availableStairsElements;
    }

    public BaseStairsElement CopyStairsElement(Stairs stairs, BaseStairsElement stairsElement) =>
        stairsRepository.CopyStairsElement(stairs, stairsElement);
}
