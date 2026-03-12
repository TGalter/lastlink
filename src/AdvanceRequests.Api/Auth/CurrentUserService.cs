using System.Security.Claims;
using AdvanceRequests.Application.Abstractions.Services;

namespace AdvanceRequests.Api.Auth;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public string? Role =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

    public Guid? UserId =>
        TryParseGuid(_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier));

    public Guid? CreatorId =>
        TryParseGuid(_httpContextAccessor.HttpContext?.User?.FindFirstValue("creator_id"));

    private static Guid? TryParseGuid(string? value)
        => Guid.TryParse(value, out var guid) ? guid : null;
}