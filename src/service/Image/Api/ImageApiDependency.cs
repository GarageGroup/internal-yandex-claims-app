using System;
using PrimeFuncPack;

namespace GarageGroup.Internal.Yandex.Claims;

public static class ImageApiDependency
{
    public static Dependency<IImageApi> UseImageApi(this Dependency<ImageApiOption> dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);

        return dependency.Map<IImageApi>(CreateApi);

        static ImageApi CreateApi(ImageApiOption option)
        {
            return new(option);
        }
    }
}