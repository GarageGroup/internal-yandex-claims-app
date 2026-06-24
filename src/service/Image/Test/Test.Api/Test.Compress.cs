using System;
using GarageGroup.Internal.Yandex.Claims.Image.Test.Source.Api;
using VipsEnums = NetVips.Enums;
using VipsImage = NetVips.Image;
using Xunit;

namespace GarageGroup.Internal.Yandex.Claims.Image.Test;

partial class ImageApiTest
{
    [Fact]
    public static void CompressImage_InputImageDataIsEmpty_ExpectInvalidFailure()
    {
        var api = new ImageApi(SomeOption);

        var actual = api.CompressImage(
            new ImageCompressIn
            {
                ImageData = []
            });

        var expected = Failure.Create("Image content must not be empty.");

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ImageApiSource.CompressWithinTargetTestData), MemberType = typeof(ImageApiSource))]
    internal static void CompressImage_InputImageBase64LengthIsWithinTarget_ExpectSuccess(
        ImageCompressIn input, ImageCompressOut expected)
    {
        var api = new ImageApi(SomeOption);

        var actual = api.CompressImage(input);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ImageApiSource.CompressSuccessTestData), MemberType = typeof(ImageApiSource))]
    internal static void CompressImage_InputImageRequiresCompression_ExpectSuccess(
        ImageCompressIn input, ImageApiOption option, int expectedWidth, int expectedHeight)
    {
        var api = new ImageApi(option);

        var actual = api.CompressImage(input);

        var actualBase64 = actual.SuccessOrThrow().Base64Image;

        Assert.True(actualBase64.Length < option.TargetBase64Length);
        Assert.NotEqual(Convert.ToBase64String(input.ImageData), actualBase64);

        using var outputImage = VipsImage.NewFromBuffer(
            Convert.FromBase64String(actualBase64),
            access: VipsEnums.Access.Random);

        Assert.Equal(expectedWidth, outputImage.Width);
        Assert.Equal(expectedHeight, outputImage.Height);
    }

    [Theory]
    [MemberData(nameof(ImageApiSource.CompressFailureTestData), MemberType = typeof(ImageApiSource))]
    internal static void CompressImage_ImageCannotBeReducedUnderTarget_ExpectFailure(
        ImageCompressIn input, ImageApiOption option, Failure<Unit> expected)
    {
        var api = new ImageApi(option);

        var actual = api.CompressImage(input);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ImageApiSource.InvalidImageDataTestData), MemberType = typeof(ImageApiSource))]
    internal static void CompressImage_InputImageDataIsInvalid_ExpectVipsException(
        ImageCompressIn input)
    {
        var api = new ImageApi(SomeOption);

        _ = Assert.Throws<NetVips.VipsException>(
            () => api.CompressImage(input));
    }
}
