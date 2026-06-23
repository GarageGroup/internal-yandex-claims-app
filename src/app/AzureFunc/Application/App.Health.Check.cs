using GarageGroup.Infra;
using PrimeFuncPack;

namespace GarageGroup.Internal.Yandex.Claims;

partial class Application
{
    [HttpFunction("HealthCheck", HttpMethodName.Get, Route = "health", AuthLevel = HttpAuthorizationLevel.Function)]
    internal static Dependency<IHealthCheckHandler> UseHealthCheckHandler()
        =>
        HealthCheck.UseServices(
            UseGraphApi().UseServiceHealthCheckApi("GraphApi"))
        .UseHealthCheckHandler();
}