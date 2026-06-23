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
    public static async Task GetProfileAvatarAsync_ExpectHttpApiCalledOnce()
    {
        var httpApi = BuildHttpApi(SomeSuccessOutput);
        var api = new GraphApi(httpApi);

        var input = new ProfileAvatarGetIn
        {
            UserId = new Guid("9f1db8cc-0b3c-4f9e-8d66-3c3b0d6ce201")
        };

        _ = await api.GetProfileAvatarAsync(input, TestContext.Current.CancellationToken);

        var expectedInput = new HttpSendIn(
            method: HttpVerb.Get,
            requestUri: "users/9f1db8cc-0b3c-4f9e-8d66-3c3b0d6ce201/photo/$value");

        _ = await httpApi.Received(1).SendAsync(expectedInput, Arg.Any<CancellationToken>());
    }

    [Theory]
    [MemberData(nameof(GraphApiSource.FailureHttpGetProfileAvatarTestData), MemberType = typeof(GraphApiSource))]
    public static async Task GetProfileAvatarAsync_HttpResultIsFailure_ExpectFailure(
        HttpSendFailure httpFailure, Failure<Unit> expected)
    {        
        var httpApi = BuildHttpApi(httpFailure);
        var api = new GraphApi(httpApi);

        var actual = await api.GetProfileAvatarAsync(SomeInput, TestContext.Current.CancellationToken);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GraphApiSource.SuccessHttpGetProfileAvatarTestData), MemberType = typeof(GraphApiSource))]
    public static async Task GetProfileAvatarAsync_HttpResultIsSuccess_ExpectSuccess(
        HttpSendOut httpSuccess, ProfileAvatarGetOut expected)
    {
        var httpApi = BuildHttpApi(httpSuccess);
        var api = new GraphApi(httpApi);

        var actual = await api.GetProfileAvatarAsync(SomeInput, TestContext.Current.CancellationToken);

        Assert.Equal(expected.Image, actual.SuccessOrThrow().Image);
    }
}
