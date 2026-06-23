using System;

namespace GarageGroup.Internal.Yandex.Claims;

public sealed record class ProfileAvatarGetIn
{
    public Guid UserId { get; init; }
}
