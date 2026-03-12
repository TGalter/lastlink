namespace AdvanceRequests.Api.Contracts.Auth;

public sealed class LoginRequest
{
    public string Role { get; init; } = string.Empty;
    public Guid? CreatorId { get; init; }
}