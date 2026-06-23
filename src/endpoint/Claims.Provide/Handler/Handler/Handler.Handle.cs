using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

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
            static @out => new ImageCompressIn
            {
                ImageData = @out.Image
            },
            static failure => failure.WithFailureCode(HandlerFailureCode.Persistent))
        .Forward(
            imageApi.CompressAsync,
            static failure => failure.WithFailureCode(HandlerFailureCode.Persistent))
        .MapSuccess(
            success => new ClaimsProvideOut(
                data: new()
                {
                    Actions =
                    [
                        new(
                            claims: new(
                                avatar: success.Base64Image))
                    ]
                }));
}