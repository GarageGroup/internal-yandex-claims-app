using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using GarageGroup.Internal.Yandex.Claims.GraphApiTests.Source.Api;
using NSubstitute;
using Xunit;

namespace GarageGroup.Internal.Yandex.Claims.GraphApiTests;

partial class GraphApiTest
{
    [Fact]
    public static async Task PingAsync_ExpectHttpApiCalledOnce()
    {
        var httpApi = BuildHttpApi(SomeSuccessOutput);
        var api = new GraphApi(httpApi);

        _ = await api.PingAsync(default, TestContext.Current.CancellationToken);

        var expectedInput = new HttpSendIn(
            method: HttpVerb.Get,
            requestUri: "users?$top=1&$select=id")
        {
            SuccessType = HttpSuccessType.OnlyStatusCode
        };

        _ = await httpApi.Received(1).SendAsync(expectedInput, Arg.Any<CancellationToken>());
    }

    [Theory]
    [MemberData(nameof(GraphApiSource.FailureHttpPingTestData), MemberType = typeof(GraphApiSource))]
    public static async Task PingAsync_HttpResultIsFailure_ExpectFailure(
        HttpSendFailure httpFailure, Failure<Unit> expected)
    {
        var httpApi = BuildHttpApi(httpFailure);
        var api = new GraphApi(httpApi);

        var actual = await api.PingAsync(default, TestContext.Current.CancellationToken);
        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task PingAsync_HttpResultIsSuccess_ExpectSuccess()
    {
        var httpApi = BuildHttpApi(SomeSuccessOutput);
        var api = new GraphApi(httpApi);

        var actual = await api.PingAsync(default, TestContext.Current.CancellationToken);
        var expected = Result.Success<Unit>(default);

        Assert.StrictEqual(expected, actual);
    }
}
