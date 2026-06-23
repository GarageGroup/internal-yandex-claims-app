using System;

namespace GarageGroup.Internal.Yandex.Claims;

public sealed record class User
{
    public Guid Id { get; init; }
}