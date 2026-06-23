using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrimeFuncPack;

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