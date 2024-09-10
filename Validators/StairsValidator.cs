using FluentValidation;

namespace FireEscape.Validators;

public class StairsValidator : AbstractValidator<Stairs>
{
    public StairsValidator()
    {
        RuleFor(stairs => stairs.StairsWidth.Value).GreaterThan(0).WithMessage(AppResources.StairsWidthHint);
        RuleFor(stairs => stairs.StairsHeight.Value).GreaterThan(0).WithMessage(AppResources.StairsHeightHint);

        When(stairs => stairs.StairsType is StairsTypeEnum.P1_1 or StairsTypeEnum.P1_2, () => // check stairs elements for P1 & P1-2
        {
            RuleForEach(stairs => stairs.StairsElements).Where(stairsElement => stairsElement.StairsElementType == typeof(SupportBeamsP1)).
                Must(stairsElement => ((SupportBeamsP1)stairsElement).SupportBeamsCount > 0).
                WithMessage(AppResources.SupportBeamsPairsCountHint).
                Must(stairsElement => ((SupportBeamsP1)stairsElement).WallDistance.Value > 0).
                WithMessage(AppResources.WallDistanceHint);

            RuleFor(stairs => stairs.StepsCount).GreaterThan(0).WithMessage(AppResources.StepsCountHint);

            RuleForEach(stairs => stairs.StairsElements).Where(stairsElement => stairsElement.StairsElementType == typeof(PlatformP1)).
                Must(stairsElement => ((PlatformP1)stairsElement).PlatformLength.Value > 0).
                WithMessage(AppResources.PlatformLengthHint).
                Must(stairsElement => ((PlatformP1)stairsElement).PlatformWidth.Value > 0).
                WithMessage(AppResources.PlatformWidthHint).
                Must(stairsElement => ((PlatformP1)stairsElement).SupportBeamsCount > 0).
                WithMessage(AppResources.PlatformSupportBeamsCountHint);

            RuleForEach(stairs => stairs.StairsElements).Where(stairsElement => stairsElement.StairsElementType == typeof(StepsP1)).
                Must(stairsElement => ((StepsP1)stairsElement).StepsDistance.Value > 0).
                WithMessage(AppResources.StepsDistanceHint);

            When(stairs => stairs.StairsType is StairsTypeEnum.P1_2, () => // check is fence exists for P1-2
            {
                RuleFor(stairs => stairs.StairsElements.Where(stairsElement => stairsElement.StairsElementType == typeof(FenceP1)).ToArray()).
                    Must(stairsElements => stairsElements.Length == 1)
                    .WithMessage(AppResources.FenceAbsentHint);
            });
        });


        When(stairs => stairs.StairsType is StairsTypeEnum.P2, () => // check stairs elements for P2
        {
            RuleForEach(stairs => stairs.StairsElements).Where(stairsElement => stairsElement.StairsElementType == typeof(PlatformP2)).
                Must(stairsElement => ((PlatformP2)stairsElement).PlatformLength.Value > 0).
                WithMessage(AppResources.PlatformLengthHint).
                Must(stairsElement => ((PlatformP2)stairsElement).PlatformWidth.Value > 0).
                WithMessage(AppResources.PlatformWidthHint).
                Must(stairsElement => ((PlatformP2)stairsElement).SupportBeamsCount > 0).
                WithMessage(AppResources.PlatformSupportBeamsCountHint);

            RuleForEach(stairs => stairs.StairsElements).Where(stairsElement => stairsElement.StairsElementType == typeof(StairwayP2)).
                Must(stairsElement => ((StairwayP2)stairsElement).StepsCount > 0).
                WithMessage(AppResources.StairwayStepsCountHint).
                Must(stairsElement => ((StairwayP2)stairsElement).StairwayLength > 0).
                WithMessage(AppResources.StairwayLengthHint).
                Must(stairsElement => ((StairwayP2)stairsElement).SupportBeamsCount > 0).
                WithMessage(AppResources.StairwaySupportBeamsCountHint);

            RuleForEach(stairs => stairs.StairsElements).Where(stairsElement => stairsElement.StairsElementType == typeof(StepsP2)).
                Must(stairsElement => ((StepsP2)stairsElement).StepsWidth.Value > 0).
                WithMessage(AppResources.StepsWidthHint).
                Must(stairsElement => ((StepsP2)stairsElement).StepsHeight.Value > 0).
                WithMessage(AppResources.StepsHeightHint);

            RuleForEach(stairs => stairs.StairsElements).Where(stairsElement => stairsElement.StairsElementType == typeof(FenceP2)).
                Must(stairsElement => ((FenceP2)stairsElement).FenceHeight.Value > 0).
                WithMessage(AppResources.StairsFenceHeightHint);

            RuleFor(stairs => stairs.StairsElements.Where(stairsElement => stairsElement.StairsElementType == typeof(PlatformP2)).ToArray()).
                Must(stairsElements => stairsElements.Length > 0).WithMessage(AppResources.PlatformAbsentHint);

            RuleFor(stairs => stairs.StairsElements.Where(stairsElement => stairsElement.StairsElementType == typeof(StairwayP2)).ToArray()).
                Must(stairsElements => stairsElements.Length > 0).WithMessage(AppResources.StairwayAbsentHint);
        });
    }
}