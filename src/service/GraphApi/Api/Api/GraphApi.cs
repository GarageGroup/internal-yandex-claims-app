using GarageGroup.Infra;

namespace GarageGroup.Internal.Yandex.Claims;

public sealed partial class GraphApi(IHttpApi httpApi) : IGraphApi
{
    private static HttpSendIn PingHttpSendInput
        =>
        new(
            method: HttpVerb.Get,
            requestUri: "users?$top=1&$select=id")
        {
            SuccessType = HttpSuccessType.OnlyStatusCode
        };
}
