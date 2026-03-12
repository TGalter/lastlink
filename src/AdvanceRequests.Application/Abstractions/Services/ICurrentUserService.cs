namespace AdvanceRequests.Application.Abstractions.Services;

public interface ICurrentUserService
{
    string UserId { get; }
    string Role { get; }
}