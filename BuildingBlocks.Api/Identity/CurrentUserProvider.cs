using System.Security.Claims;
using BuildingBlocks.Application.Identity;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Api.Identity;

public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    public Guid UserId
    {
        get
        {
            var claim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

            return Guid.Parse(claim.Value);
        }
    }
}