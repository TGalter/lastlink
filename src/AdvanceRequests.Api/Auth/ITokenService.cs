namespace AdvanceRequests.Api.Auth;

public interface ITokenService
{
    (string AccessToken, DateTime ExpiresAtUtc) GenerateAdminToken();
    (string AccessToken, DateTime ExpiresAtUtc) GenerateCreatorToken(Guid creatorId);
}