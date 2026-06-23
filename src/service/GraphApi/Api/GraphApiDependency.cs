using System;
using GarageGroup.Infra;
using PrimeFuncPack;

namespace GarageGroup.Internal.Yandex.Claims;

public static class GraphApiDependency
{
    public static Dependency<IGraphApi> UseGraphApi(this Dependency<IHttpApi> dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);

        return dependency.Map<IGraphApi>(CreateApi);

        static GraphApi CreateApi(IHttpApi httpApi)
        {
            return new(httpApi);
        }
    }
}