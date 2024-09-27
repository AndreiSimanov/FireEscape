using FluentValidation.Results;

namespace FireEscape.Services.Interfaces;

public interface IStairsService
{
    BaseStairsElement CopyStairsElement(Stairs stairs, BaseStairsElement stairsElement);
    IEnumerable<BaseStairsElement> GetAvailableStairsElements(Stairs stairs);
    Task SaveAsync(Stairs stairs);
    ValidationResult Validate(Stairs stairs);
}