using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using PrimeFuncPack;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Yandex.Claims.Provide.Test")]

namespace GarageGroup.Internal.Yandex.Claims;

public static class ClaimsProvideDependency
{
    public static Dependency<IClaimsProvideHandler> UseClaimsProvideHandler(this Dependency<IImageApi, IGraphApi, ILoggerFactory> dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Fold<IClaimsProvideHandler>(CreateHandler);

        static ClaimsProvideHandler CreateHandler(IImageApi imageApi, IGraphApi graphApi, ILoggerFactory loggerFactory)
        {
            ArgumentNullException.ThrowIfNull(imageApi);
            ArgumentNullException.ThrowIfNull(graphApi);
            ArgumentNullException.ThrowIfNull(loggerFactory);
            
            return new(imageApi, graphApi, loggerFactory.CreateLogger<ClaimsProvideHandler>());
        }
    }
}
