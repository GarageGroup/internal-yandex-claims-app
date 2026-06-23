using System;
using Xunit;

namespace GarageGroup.Internal.Yandex.Claims.Image.Test.Source.Api;

partial class ImageApiSource
{
    public static TheoryData<ImageCompressIn, ImageApiOption, Failure<Unit>> CompressFailureTestData
        =>
        new()
        {
            {
                new()
                {
                    ImageData = BuildNoisyPngImageData(16, 16)
                },
                new()
                {
                    TargetBase64Length = 1,
                    InitialMaxSide = 2,
                    InitialQuality = 10,
                    MinimumQuality = 10,
                    MinimumMaxSide = 2,
                    QualityStep = 10,
                    MaxSideScaleFactor = 0.5
                },
                Failure.Create("Cannot compress image to a base64 length under 1 characters.")
            }
        };
}
