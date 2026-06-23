using Xunit;

namespace GarageGroup.Internal.Yandex.Claims.Image.Test.Source.Api;

partial class ImageApiSource
{
    public static TheoryData<ImageCompressIn> InvalidImageDataTestData
        =>
        new()
        {
            new TheoryDataRow<ImageCompressIn>(
                new()
                {
                    ImageData = BuildZeroDimensionSvgImageData()
                })
        };
}
