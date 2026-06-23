using System.Text.Json.Serialization;

namespace GarageGroup.Internal.Yandex.Claims;

public sealed record class TokenIssuanceAction
{
    public TokenIssuanceAction(UserClaims claims)
        =>
        Claims = claims;

    [JsonPropertyName("@odata.type")]
    public string ODataType { get; } = "microsoft.graph.tokenIssuanceStart.provideClaimsForToken";
    
    [JsonPropertyName("claims")]
    public UserClaims Claims { get; }
}