using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Yandex.Claims;

partial class GraphApi
{
    public ValueTask<Result<ProfileAvatarGetOut, Failure<Unit>>> GetProfileAvatarAsync(
        ProfileAvatarGetIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input.UserId, cancellationToken)
        .Pipe(
            static userId => new HttpSendIn(
                method: HttpVerb.Get,
                requestUri: $"users/{userId}/photo/$value"))
        .PipeValue(
            httpApi.SendAsync)
        .Recover(
            static failure => failure.StatusCode switch
            {
                HttpFailureCode.NotFound => Result.Success<ProfileAvatarGetOut>(default),
                _ => failure
            },
            static success => new ProfileAvatarGetOut
            {
                Image = success.Body.Content?.ToArray()
            })
        .MapFailure(
            static failure => failure.ToStandardFailure().WithFailureCode<Unit>(default));
}
