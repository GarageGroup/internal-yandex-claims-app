using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using GarageGroup.Internal.Yandex.Claims.Provide.Test.Source.Handler;
using NSubstitute;
using Xunit;

namespace GarageGroup.Internal.Yandex.Claims.Provide.Test;

partial class ClaimsProvideHandlerTest
{
    [Theory]
    [MemberData(nameof(ClaimsProvideHandlerSource.GraphGetProfileAvatarInputTestData), MemberType = typeof(ClaimsProvideHandlerSource))]
    internal static async Task HandleRequiredAsync_ExpectGraphGetProfileAvatarCalledOnce(
        ClaimsProvideIn input, ProfileAvatarGetIn expectedInput)
    {
        var graphApi = BuildGraphApi(SomeProfileAvatarGetOutput);
        var imageApi = BuildImageApi(SomeImageCompressOutput);
        var handler = new ClaimsProvideHandler(imageApi, graphApi);

        _ = await handler.HandleRequiredAsync(input, TestContext.Current.CancellationToken);

        _ = await graphApi.Received(1).GetProfileAvatarAsync(expectedInput, Arg.Any<CancellationToken>());
    }

    [Fact]
    public static async Task HandleRequiredAsync_GraphGetProfileAvatarResultIsFailure_ExpectEmptySuccess()
    {
        var sourceException = new Exception("Some exception");
        var sourceFailure = Failure.Create("Some failure message", sourceException);

        var graphApi = BuildGraphApi(sourceFailure);
        var imageApi = BuildImageApi(SomeImageCompressOutput);
        var handler = new ClaimsProvideHandler(imageApi, graphApi);

        var actual = await handler.HandleRequiredAsync(SomeInput, TestContext.Current.CancellationToken);
        var expected = Result.Success(
            new ClaimsProvideOut(
                data: new()
                {
                    Actions =
                    [
                        new(
                            claims: new(
                                avatar: string.Empty))
                    ]
                }));

        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task HandleRequiredAsync_GraphGetProfileAvatarResultIsFailure_ExpectImageCompressCalledNever()
    {
        var sourceFailure = Failure.Create("Some failure message");

        var graphApi = BuildGraphApi(sourceFailure);
        var imageApi = BuildImageApi(SomeImageCompressOutput);
        var handler = new ClaimsProvideHandler(imageApi, graphApi);

        _ = await handler.HandleRequiredAsync(SomeInput, TestContext.Current.CancellationToken);

        _ = imageApi.DidNotReceive().CompressAsync(Arg.Any<ImageCompressIn>());
    }

    [Fact]
    public static async Task HandleRequiredAsync_GraphGetProfileAvatarResultIsSuccess_ExpectImageCompressCalledOnce()
    {
        var profileAvatarImage = new byte[] { 0x21, 0x22, 0x23, 0x24 };

        var graphApi = BuildGraphApi(
            new ProfileAvatarGetOut
            {
                Image = profileAvatarImage
            });

        var imageApi = BuildImageApi(SomeImageCompressOutput);
        var handler = new ClaimsProvideHandler(imageApi, graphApi);

        _ = await handler.HandleRequiredAsync(SomeInput, TestContext.Current.CancellationToken);

        var expectedInput = new ImageCompressIn
        {
            ImageData = profileAvatarImage
        };

        _ = imageApi.Received(1).CompressAsync(expectedInput);
    }

    [Fact]
    public static async Task HandleRequiredAsync_ImageCompressResultIsFailure_ExpectEmptySuccess()
    {
        var sourceException = new Exception("Some exception");
        var sourceFailure = Failure.Create("Some failure message", sourceException);

        var graphApi = BuildGraphApi(SomeProfileAvatarGetOutput);
        var imageApi = BuildImageApi(sourceFailure);
        var handler = new ClaimsProvideHandler(imageApi, graphApi);

        var actual = await handler.HandleRequiredAsync(SomeInput, TestContext.Current.CancellationToken);
        var expected = Result.Success(
            new ClaimsProvideOut(
                data: new()
                {
                    Actions =
                    [
                        new(
                            claims: new(
                                avatar: string.Empty))
                    ]
                }));
                
        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task HandleRequiredAsync_ImageCompressResultIsSuccess_ExpectSuccess()
    {
        var imageCompressOutput = new ImageCompressOut
        {
            Base64Image = "Expected base64 avatar"
        };

        var graphApi = BuildGraphApi(SomeProfileAvatarGetOutput);
        var imageApi = BuildImageApi(imageCompressOutput);
        var handler = new ClaimsProvideHandler(imageApi, graphApi);

        var actual = await handler.HandleRequiredAsync(SomeInput, TestContext.Current.CancellationToken);

        var expected = Result.Success(
            new ClaimsProvideOut(
                data: new()
                {
                    Actions =
                    [
                        new(
                            claims: new(
                                avatar: "Expected base64 avatar"))
                    ]
                }));

        Assert.StrictEqual(expected, actual);
    }
}
