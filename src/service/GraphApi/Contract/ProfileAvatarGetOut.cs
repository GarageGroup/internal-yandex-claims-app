namespace GarageGroup.Internal.Yandex.Claims;

public readonly record struct ProfileAvatarGetOut
{
    public byte[]? Image { get; init; }
}
