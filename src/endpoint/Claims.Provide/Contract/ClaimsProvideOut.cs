namespace GarageGroup.Internal.Yandex.Claims;

public sealed record class ClaimsProvideOut
{
    public ClaimsProvideOut(AuthenticationEventResponseData data)
        =>
        Data = data;

    public AuthenticationEventResponseData Data { get; }
}