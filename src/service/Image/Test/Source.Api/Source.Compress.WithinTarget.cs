using System;
using Xunit;

namespace GarageGroup.Internal.Yandex.Claims.Image.Test.Source.Api;

partial class ImageApiSource
{
    public static TheoryData<ImageCompressIn, ImageCompressOut> CompressWithinTargetTestData
    {
        get
        {
            var imageData = BuildNoisyPngImageData(1, 1);

            return new()
            {
                {
                    new()
                    {
                        ImageData = imageData
                    },
                    new()
                    {
                        Base64Image = Convert.ToBase64String(imageData)
                    }
                }
            };
        }
    }
}
