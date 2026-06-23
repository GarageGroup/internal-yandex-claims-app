namespace GarageGroup.Internal.Yandex.Claims;

public sealed record class ClaimsProvideIn
{
    public AuthenticationEventData? Data { get; init; }
}