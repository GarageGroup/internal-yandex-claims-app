using GarageGroup.Infra;
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
        .UseClaimsProvideHandler();
}