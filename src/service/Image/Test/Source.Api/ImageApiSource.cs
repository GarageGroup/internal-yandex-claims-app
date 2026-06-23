using System.Text;
using VipsEnums = NetVips.Enums;
using VipsImage = NetVips.Image;

namespace GarageGroup.Internal.Yandex.Claims.Image.Test.Source.Api;

internal static partial class ImageApiSource
{
    private static byte[] BuildNoisyPngImageData(int width, int height)
    {
        var pixelCount = checked(width * height * 3);
        var pixels = new byte[pixelCount];

        var seed = 0x12345678u;
        for (var index = 0; index < pixels.Length; index++)
        {
            seed = 1664525u * seed + 1013904223u;
            pixels[index] = (byte)(seed >> 24);
        }

        using var image = VipsImage.NewFromMemory(
            pixels,
            width,
            height,
            3,
            VipsEnums.BandFormat.Uchar);

        return image.WriteToBuffer(".png");
    }

    private static byte[] BuildZeroDimensionSvgImageData()
        =>
        Encoding.UTF8.GetBytes(
            "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"0\" height=\"0\"></svg>");
}
