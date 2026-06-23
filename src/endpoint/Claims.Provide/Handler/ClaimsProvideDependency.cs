using System;
using System.Runtime.CompilerServices;
using PrimeFuncPack;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Yandex.Claims.Provide.Test")]

namespace GarageGroup.Internal.Yandex.Claims;

public static class ClaimsProvideDependency
{
    public static Dependency<IClaimsProvideHandler> UseClaimsProvideHandler(this Dependency<IImageApi, IGraphApi> dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Fold<IClaimsProvideHandler>(CreateHandler);

        static ClaimsProvideHandler CreateHandler(IImageApi imageApi, IGraphApi graphApi)
        {
            ArgumentNullException.ThrowIfNull(imageApi);
            ArgumentNullException.ThrowIfNull(graphApi);
            
            return new(imageApi, graphApi);
        }
    }
}
