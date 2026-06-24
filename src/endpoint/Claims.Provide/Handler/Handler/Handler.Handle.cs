using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Internal.Yandex.Claims;

partial class ClaimsProvideHandler
{
    public ValueTask<Result<ClaimsProvideOut, Failure<HandlerFailureCode>>> HandleRequiredAsync(
        ClaimsProvideIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            static @in => new ProfileAvatarGetIn()
            {
                UserId = @in.Data?.AuthenticationContext?.User?.Id ?? default
            })
        .PipeValue(
            graphApi.GetProfileAvatarAsync)
        .Map(
            static @out => @out.Image,
            MapFailure)
        .Forward(
            CompressImage)
        .OnFailure(
            failure => logger.LogError(failure.SourceException, "Error: {failureMessage}", failure.FailureMessage))
        .Recover(
            static _ => EmptyClaims);

    private Result<ClaimsProvideOut, Failure<HandlerFailureCode>> CompressImage(byte[]? image)
    {
        if (image?.Length is not > 0)
        {
            return EmptyClaims;
        }

        var @in = new ImageCompressIn
        {
            ImageData = image
        };

        return imageApi.CompressImage(@in).Map(MapSuccess, MapFailure);

        static ClaimsProvideOut MapSuccess(ImageCompressOut success)
            =>
            new(
                data: new()
                {
                    Actions =
                    [
                        new(
                            claims: new(
                                avatar: success.Base64Image))
                    ]
                });
    }

    private static Failure<HandlerFailureCode> MapFailure(Failure<Unit> failure)
        =>
        failure.WithFailureCode(HandlerFailureCode.Persistent);
}