using System;

namespace GarageGroup.Internal.Yandex.Claims;

public sealed record class ProfileAvatarGetOut
{
    public required byte[] Image { get; init; }
}
