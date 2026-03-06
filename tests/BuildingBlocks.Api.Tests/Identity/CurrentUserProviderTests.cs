using System.Security.Claims;
using BuildingBlocks.Api.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace BuildingBlocks.Api.Tests.Identity;

public class CurrentUserProviderTests
{
    [Fact]
    public void UserId_WithValidClaim_ReturnsGuid()
    {
        var userId = Guid.NewGuid();
        var httpContextAccessor = CreateHttpContextAccessor(userId);
        var provider = new CurrentUserProvider(httpContextAccessor);

        provider.UserId.Should().Be(userId);
    }

    [Fact]
    public void UserId_WithoutClaim_ThrowsUnauthorizedAccessException()
    {
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(new DefaultHttpContext());
        var provider = new CurrentUserProvider(httpContextAccessor);

        var act = () => provider.UserId;

        act.Should().Throw<UnauthorizedAccessException>();
    }

    [Fact]
    public void UserId_WithNullHttpContext_ThrowsUnauthorizedAccessException()
    {
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns((HttpContext?)null);
        var provider = new CurrentUserProvider(httpContextAccessor);

        var act = () => provider.UserId;

        act.Should().Throw<UnauthorizedAccessException>();
    }

    private static IHttpContextAccessor CreateHttpContextAccessor(Guid userId)
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(httpContext);
        return accessor;
    }
}
