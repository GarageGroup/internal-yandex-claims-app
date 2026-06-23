namespace GarageGroup.Internal.Yandex.Claims;

public sealed record class ImageCompressIn
{
    public required byte[] ImageData { get; init; }
}