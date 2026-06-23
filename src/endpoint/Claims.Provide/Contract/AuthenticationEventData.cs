namespace GarageGroup.Internal.Yandex.Claims;

public sealed record class AuthenticationEventData
{
    public AuthenticationContext? AuthenticationContext { get; init; }
}