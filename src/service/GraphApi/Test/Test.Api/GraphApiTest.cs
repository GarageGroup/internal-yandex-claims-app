using System;
using System.Threading;
using GarageGroup.Infra;
using NSubstitute;

namespace GarageGroup.Internal.Yandex.Claims.GraphApiTests;

public static partial class GraphApiTest
{
    private static readonly HttpSendOut SomeSuccessOutput
        =
        new()
        {
            StatusCode = HttpSuccessCode.OK
        };

    private static readonly ProfileAvatarGetIn SomeInput
        =
        new()
        {
            UserId = new Guid("9f1db8cc-0b3c-4f9e-8d66-3c3b0d6ce201")
        };

    private static IHttpApi BuildHttpApi(in Result<HttpSendOut, HttpSendFailure> result)
    {
        var api = Substitute.For<IHttpApi>();
        api.SendAsync(Arg.Any<HttpSendIn>(), Arg.Any<CancellationToken>()).Returns(result);
        return api;
    }
}
