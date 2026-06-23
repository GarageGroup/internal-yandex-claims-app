namespace GarageGroup.Internal.Yandex.Claims;

public sealed record class ImageApiOption
{
    public int TargetBase64Length { get; init; }

    public int InitialMaxSide { get; init; }

    public int InitialQuality { get; init; }

    public int MinimumQuality { get; init; }

    public int MinimumMaxSide { get; init; }

    public int QualityStep { get; init; }

    public double MaxSideScaleFactor { get; init; }
}
