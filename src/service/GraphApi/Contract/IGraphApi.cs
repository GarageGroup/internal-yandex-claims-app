using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Yandex.Claims;

public interface IGraphApi : IPingSupplier
{
    ValueTask<Result<ProfileAvatarGetOut, Failure<Unit>>> GetProfileAvatarAsync(
        ProfileAvatarGetIn input, CancellationToken cancellationToken);
}
