using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Yandex.Claims;

partial class GraphApi
{
    public ValueTask<Result<Unit, Failure<Unit>>> PingAsync(
        Unit input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            static userId => new HttpSendIn(
                method: HttpVerb.Get,
                requestUri: "me")
            {
                SuccessType = HttpSuccessType.OnlyStatusCode
            })
        .PipeValue(
            httpApi.SendAsync)
        .Map(
            Unit.From,
            static failure => failure.ToStandardFailure().WithFailureCode<Unit>(default));
}
