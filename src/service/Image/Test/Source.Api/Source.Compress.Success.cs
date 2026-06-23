using Xunit;

namespace GarageGroup.Internal.Yandex.Claims.Image.Test.Source.Api;

partial class ImageApiSource
{
    public static TheoryData<ImageCompressIn, ImageApiOption, int, int> CompressSuccessTestData
        =>
        new()
        {
            {
                new()
                {
                    ImageData = BuildNoisyPngImageData(64, 64)
                },
                new()
                {
                    TargetBase64Length = 3000,
                    InitialMaxSide = 32,
                    InitialQuality = 10,
                    MinimumQuality = 10,
                    MinimumMaxSide = 2,
                    QualityStep = 10,
                    MaxSideScaleFactor = 0.5
                },
                32,
                32
            },
            {
                new()
                {
                    ImageData = BuildNoisyPngImageData(64, 64)
                },
                new()
                {
                    TargetBase64Length = 3000,
                    InitialMaxSide = 64,
                    InitialQuality = 80,
                    MinimumQuality = 10,
                    MinimumMaxSide = 2,
                    QualityStep = 10,
                    MaxSideScaleFactor = 0.5
                },
                64,
                64
            }
        };
}
