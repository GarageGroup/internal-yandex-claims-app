using GarageGroup.Infra;

namespace GarageGroup.Internal.Yandex.Claims;

public interface IClaimsProvideHandler : IRequiredHandler<ClaimsProvideIn, ClaimsProvideOut>;