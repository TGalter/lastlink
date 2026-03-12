namespace AdvanceRequests.Application.Abstractions.Services;
public interface ICurrentUserService
{
    bool IsAuthenticated { get; }
    string? Role { get; }
    Guid? UserId { get; }
    Guid? CreatorId { get; }
}