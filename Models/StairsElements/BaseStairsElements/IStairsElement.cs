namespace FireEscape.Models.StairsElements.BaseStairsElements;

public interface IStairsElement
{
    string Name { get; }
    string Caption { get; }
    bool Required { get; }

    StairsElementTypeEnum StairsElementType { get; }
    BaseStairsTypeEnum BaseStairsType { get; }

    int StairsStepsCount { get; set; }
    float StairsHeight { get; set; }

    float CalcWithstandLoad { get; }
    int TestPointCount { get; }
    int ElementNumber { get; set; }
    int PrintOrder { get; set; }
    int SupportBeamsCount { get; set; }

    int ElementStepsCount { get { return 0; } set { } }

    float ElementHeight { get; set; }
    float ElementWidth { get; set; }

    float WithstandLoad { get; set; }

    ServiceabilityProperty<float> Deformation { get; set; }


    // todo: remove it from BaseStairsElement!

    string StepsCountCaption => string.Empty;


    string StepsCountHint => string.Empty;


    string SupportBeamsCountCaption => string.Empty;


    string SupportBeamsCountHint => string.Empty;


    string ElementHeightCaption => string.Empty;


    string ElementHeightHint => string.Empty;


    string ElementWidthCaption => string.Empty;


    string ElementWidthHint => string.Empty;
}
