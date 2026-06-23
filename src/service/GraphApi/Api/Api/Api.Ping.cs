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
            static _ => new HttpSendIn(
                method: HttpVerb.Get,
                requestUri: "users?$top=1&$select=id")
            {
                SuccessType = HttpSuccessType.OnlyStatusCode
            })
        .PipeValue(
            httpApi.SendAsync)
        .Map(
            Unit.From,
            static failure => failure.ToStandardFailure().WithFailureCode<Unit>(default));
}
