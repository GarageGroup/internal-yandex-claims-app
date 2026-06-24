using Xunit;

namespace GarageGroup.Internal.Yandex.Claims.Provide.Test.Source.Handler;

partial class ClaimsProvideHandlerSource
{
    public static TheoryData<ProfileAvatarGetOut> EmptyProfileAvatarGetOutputTestData
        =>
        new()
        {
            new(
                default),
            new(
                new()
                {
                    Image = []
                })
        };
}
