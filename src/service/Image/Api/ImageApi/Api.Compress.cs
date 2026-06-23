using System;
using NetVips;

namespace GarageGroup.Internal.Yandex.Claims;

partial class ImageApi
{
    public Result<ImageCompressOut, Failure<Unit>> CompressAsync(ImageCompressIn input)
    {
        var imageContent = input.ImageData;
        if (imageContent.Length is 0)
        {
            return Failure.Create("Image content must not be empty.");
        }

        using var source = Image.NewFromBuffer(imageContent, access: Enums.Access.Random);

        if (GetBase64Length(imageContent.Length) <= option.TargetBase64Length)
        {
            return new ImageCompressOut
            {
                Base64Image = Convert.ToBase64String(imageContent)
            };
        }

        if (source.Width <= 0 || source.Height <= 0)
        {
            return Failure.Create("Failed to determine source image dimensions.");
        }

        var maxSide = option.InitialMaxSide;
        var quality = option.InitialQuality;
        var sourceMaxSide = Math.Max(source.Width, source.Height);

        while (true)
        {
            var scale = Math.Min(1.0, (double)maxSide / sourceMaxSide);

            using var resized = source
                .Resize(scale)
                .Colourspace(Enums.Interpretation.Srgb)
                .Copy();

            var memory = resized.WriteToMemory<byte>();

            using var clean = Image.NewFromMemory(
                memory,
                resized.Width,
                resized.Height,
                resized.Bands,
                resized.Format);

            var jpegBytes = clean.JpegsaveBuffer(q: quality);
            if (GetBase64Length(jpegBytes.Length) <= option.TargetBase64Length)
            {
                return new ImageCompressOut
                {
                    Base64Image = Convert.ToBase64String(jpegBytes)
                };
            }

            if (quality > option.MinimumQuality)
            {
                quality = Math.Max(option.MinimumQuality, quality - option.QualityStep);
                continue;
            }

            maxSide = Math.Max(1, (int)Math.Floor(maxSide * option.MaxSideScaleFactor));
            quality = option.InitialQuality;

            if (maxSide < option.MinimumMaxSide)
            {
                return Failure.Create(
                    $"Cannot compress image to a base64 length under {option.TargetBase64Length} characters.");
            }
        }
    }

    private static long GetBase64Length(int byteCount)
        =>
        ((byteCount + 2L) / 3L) * 4L;
}
