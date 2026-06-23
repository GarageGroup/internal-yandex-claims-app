namespace GarageGroup.Internal.Yandex.Claims.Image.Test;

public static partial class ImageApiTest
{
    private static readonly ImageApiOption SomeOption
        =
        new()
        {
            TargetBase64Length = 3000,
            InitialMaxSide = 32,
            InitialQuality = 10,
            MinimumQuality = 10,
            MinimumMaxSide = 2,
            QualityStep = 10,
            MaxSideScaleFactor = 0.5
        };
}
