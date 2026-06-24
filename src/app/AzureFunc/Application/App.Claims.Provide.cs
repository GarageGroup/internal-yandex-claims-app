using GarageGroup.Infra;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PrimeFuncPack;

namespace GarageGroup.Internal.Yandex.Claims;

partial class Application
{
    [HttpFunction("ClaimsProvide", HttpMethodName.Post, Route = "claims/provide", AuthLevel = HttpAuthorizationLevel.Function)]
    internal static Dependency<IClaimsProvideHandler> ProvideClaimsAsync()
        =>
        Dependency.Pipe(
            UseImageApi())
        .With(
            UseGraphApi())
        .With(
            ServiceProviderServiceExtensions.GetRequiredService<ILoggerFactory>)
        .UseClaimsProvideHandler();
}