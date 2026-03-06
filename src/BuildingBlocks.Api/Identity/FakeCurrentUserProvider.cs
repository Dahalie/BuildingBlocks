using BuildingBlocks.Application.Identity;

namespace BuildingBlocks.Api.Identity;

public class FakeCurrentUserProvider : ICurrentUserProvider
{
    // Sabit geliştirme kullanıcısı
    public Guid UserId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000001");
}