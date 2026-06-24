using System;
using GarageGroup.Infra;
using Xunit;

namespace GarageGroup.Internal.Yandex.Claims.GraphApiTests.Source.Api;

partial class GraphApiSource
{
    public static TheoryData<HttpSendOut, ProfileAvatarGetOut> SuccessHttpGetProfileAvatarTestData
        =>
        new()
        {
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK,
                    Body = new()
                    {
                        Content = BinaryData.FromBytes([0x11, 0x12, 0x13, 0x14])
                    }
                },
                new()
                {
                    Image = [0x11, 0x12, 0x13, 0x14]
                }
            },
            {
                new()
                {
                    StatusCode = HttpSuccessCode.OK
                },
                new()
                { }
            }
        };
}
