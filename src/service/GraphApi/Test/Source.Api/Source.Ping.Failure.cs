using System;
using GarageGroup.Infra;
using Xunit;

namespace GarageGroup.Internal.Yandex.Claims.GraphApiTests.Source.Api;

partial class GraphApiSource
{
    public static TheoryData<HttpSendFailure, Failure<Unit>> FailureHttpPingTestData
        =>
        new()
        {
            {
                default,
                Failure.Create("An unexpected http failure occured: 0.")
            },
            {
                new()
                {
                    StatusCode = HttpFailureCode.NotFound,
                    Body = new()
                    {
                        Content = BinaryData.FromString("Some failure message")
                    }
                },
                Failure.Create("An unexpected http failure occured: 404.\nSome failure message")
            },
            {
                new()
                {
                    StatusCode = HttpFailureCode.InternalServerError,
                    ReasonPhrase = "Some reason",
                    Body = new()
                    {
                        Content = BinaryData.FromString("Some error text.")
                    }
                },
                Failure.Create("An unexpected http failure occured: 500 Some reason.\nSome error text.")
            }
        };
}
