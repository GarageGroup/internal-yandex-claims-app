using System;
using GarageGroup.Infra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrimeFuncPack;

[assembly: RefreshableTokenCredential("0 */30 * * * *", IsDisabled = false)]

namespace GarageGroup.Internal.Yandex.Claims;

internal static partial class Application
{
    private static Dependency<IGraphApi> UseGraphApi()
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging("GraphApi", HttpLoggerType.RequestBody | HttpLoggerType.ResponseBody)
        .UseTokenCredentialStandard()
        .UsePollyStandard()
        .UseHttpApi("GraphApi")
        .UseGraphApi();

    private static Dependency<IImageApi> UseImageApi()
        =>
        Dependency.From(ResolveImageApiOption).UseImageApi();

    private static ImageApiOption ResolveImageApiOption(this IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var section = configuration.GetSection("ImageApiOption");

        return new()
        {
            TargetBase64Length = section.GetValue<int>("TargetBase64Length"),
            InitialMaxSide = section.GetValue<int>("InitialMaxSide"),
            InitialQuality = section.GetValue<int>("InitialQuality"),
            MinimumQuality = section.GetValue<int>("MinimumQuality"),
            MinimumMaxSide = section.GetValue<int>("MinimumMaxSide"),
            QualityStep = section.GetValue<int>("QualityStep"),
            MaxSideScaleFactor = section.GetValue<double>("MaxSideScaleFactor")
        };
    }
}