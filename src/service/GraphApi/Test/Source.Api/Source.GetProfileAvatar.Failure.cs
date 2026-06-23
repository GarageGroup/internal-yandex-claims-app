using System;
using System.Net.Mime;
using GarageGroup.Infra;
using Xunit;

namespace GarageGroup.Internal.Yandex.Claims.GraphApiTests.Source.Api;

partial class GraphApiSource
{
    public static TheoryData<HttpSendFailure, Failure<Unit>> FailureHttpGetProfileAvatarTestData
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
                        Type = new(MediaTypeNames.Application.Json),
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
                    Headers =
                    [
                        new("SomeHeader", "Some value")
                    ],
                    Body = new()
                    {
                        Content = BinaryData.FromString("Some error text.")
                    }
                },
                Failure.Create("An unexpected http failure occured: 500 Some reason.\nSome error text.")
            }
        };
}
