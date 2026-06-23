using System;
using System.Threading;
using NSubstitute;

namespace GarageGroup.Internal.Yandex.Claims.Provide.Test;

public static partial class ClaimsProvideHandlerTest
{
    private static readonly ClaimsProvideIn SomeInput
        =
        new()
        {
            Data = new()
            {
                AuthenticationContext = new()
                {
                    User = new()
                    {
                        Id = new("3f92fa34-237f-4db1-89d0-28e0fdd9987d")
                    }
                }
            }
        };

    private static readonly ProfileAvatarGetOut SomeProfileAvatarGetOutput
        =
        new()
        {
            Image = [0x11, 0x12, 0x13]
        };

    private static readonly ImageCompressOut SomeImageCompressOutput
        =
        new()
        {
            Base64Image = "Some base64 image"
        };

    private static IGraphApi BuildGraphApi(
        in Result<ProfileAvatarGetOut, Failure<Unit>> result)
    {
        var api = Substitute.For<IGraphApi>();

        api.GetProfileAvatarAsync(Arg.Any<ProfileAvatarGetIn>(), Arg.Any<CancellationToken>())
            .Returns(result);

        return api;
    }

    private static IImageApi BuildImageApi(
        in Result<ImageCompressOut, Failure<Unit>> result)
    {
        var api = Substitute.For<IImageApi>();

        api.CompressAsync(Arg.Any<ImageCompressIn>())
            .Returns(result);

        return api;
    }
}
