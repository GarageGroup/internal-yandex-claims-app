namespace GarageGroup.Internal.Yandex.Claims;

public sealed record class ImageCompressOut
{
    public required string Base64Image { get; init; }
}
