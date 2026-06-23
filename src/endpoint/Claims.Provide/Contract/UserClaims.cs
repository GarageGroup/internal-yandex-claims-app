using System;

namespace GarageGroup.Internal.Yandex.Claims;

public sealed record class UserClaims
{
    public UserClaims(string avatar)
        =>
        Avatar = avatar;

    public string Avatar { get; }
}