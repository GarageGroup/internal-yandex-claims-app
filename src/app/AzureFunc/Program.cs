using System.Threading.Tasks;
using GarageGroup.Infra;
using Microsoft.Extensions.Hosting;

namespace GarageGroup.Internal.Yandex.Claims;

static class Program
{
    static Task Main()
        =>
        FunctionHost.CreateFunctionsWorkerBuilderStandard().Build().RunAsync();
}