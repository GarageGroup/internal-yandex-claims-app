using System;
using Xunit;

namespace GarageGroup.Internal.Yandex.Claims.Provide.Test.Source.Handler;

partial class ClaimsProvideHandlerSource
{
    public static TheoryData<ClaimsProvideIn, ProfileAvatarGetIn> GraphGetProfileAvatarInputTestData
        =>
        new()
        {
            {
                new()
                {
                    Data = new()
                    {
                        AuthenticationContext = new()
                        {
                            User = new()
                            {
                                Id = new("d94fc175-abd8-4149-837b-ac6cf242e3c1")
                            }
                        }
                    }
                },
                new()
                {
                    UserId = new("d94fc175-abd8-4149-837b-ac6cf242e3c1")
                }
            },
            {
                new()
                {
                    Data = null
                },
                new()
                {
                    UserId = Guid.Empty
                }
            },
            {
                new()
                {
                    Data = new()
                    {
                        AuthenticationContext = null
                    }
                },
                new()
                {
                    UserId = Guid.Empty
                }
            },
            {
                new()
                {
                    Data = new()
                    {
                        AuthenticationContext = new()
                        {
                            User = null
                        }
                    }
                },
                new()
                {
                    UserId = Guid.Empty
                }
            }
        };
}
