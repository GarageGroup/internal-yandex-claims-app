using System;

namespace GarageGroup.Internal.Yandex.Claims;

public interface IImageApi
{
    Result<ImageCompressOut, Failure<Unit>> CompressAsync(ImageCompressIn input);
}