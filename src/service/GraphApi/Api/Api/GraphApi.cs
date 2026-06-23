using GarageGroup.Infra;

namespace GarageGroup.Internal.Yandex.Claims;

public sealed partial class GraphApi(IHttpApi httpApi) : IGraphApi;
