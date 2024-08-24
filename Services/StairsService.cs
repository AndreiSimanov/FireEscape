using FluentValidation;
using FluentValidation.Results;

namespace FireEscape.Services;

public class StairsService(IStairsRepository stairsRepository, IValidator<Stairs> validator)
{
    public Task SaveAsync(Stairs stairs) => stairsRepository.SaveAsync(stairs);

    public ValidationResult Validate(Stairs stairs) => validator.Validate(stairs);
}
