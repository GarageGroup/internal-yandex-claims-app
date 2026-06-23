using System;

namespace GarageGroup.Internal.Yandex.Claims;

public sealed record class AuthenticationContext
{
    public Guid CorrelationId { get; init; }
    
    public User? User { get; init; }
}