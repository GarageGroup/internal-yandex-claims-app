using Microsoft.Extensions.Logging;

namespace GarageGroup.Internal.Yandex.Claims;

internal sealed partial class ClaimsProvideHandler(IImageApi imageApi, IGraphApi graphApi, ILogger<ClaimsProvideHandler> logger) : IClaimsProvideHandler
{
    private static readonly ClaimsProvideOut EmptyClaims
        =
        new(
            data: new()
            {
                Actions =
                [
                    new(
                        claims: new(
                            avatar: string.Empty))
                ]
            });
}