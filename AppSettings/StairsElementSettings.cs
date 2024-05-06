﻿namespace FireEscape.AppSettings;

public record StairsElementSettings(
    BaseStairsTypeEnum BaseStairsType,
    string Type,
    int PrintOrder,
    float WithstandLoad,
    bool Required,
    int MaxCount,
    int SupportBeamsCount);
