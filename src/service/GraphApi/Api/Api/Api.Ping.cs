using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Yandex.Claims;

partial class GraphApi
{
    public ValueTask<Result<Unit, Failure<Unit>>> PingAsync(
        Unit input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            PingHttpSendInput, cancellationToken)
        .PipeValue(
            httpApi.SendAsync)
        .Map(
            Unit.From,
            static failure => failure.ToStandardFailure().WithFailureCode<Unit>(default));
}
