using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AdvanceRequests.Api.Auth;

public sealed class TokenService : ITokenService
{
    private readonly JwtOptions _options;

    public TokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public (string AccessToken, DateTime ExpiresAtUtc) GenerateAdminToken()
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "admin"),
            new(ClaimTypes.Role, "Admin")
        };

        return GenerateToken(claims);
    }

    public (string AccessToken, DateTime ExpiresAtUtc) GenerateCreatorToken(Guid creatorId)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, creatorId.ToString()),
            new(ClaimTypes.NameIdentifier, creatorId.ToString()),
            new("creator_id", creatorId.ToString()),
            new(ClaimTypes.Role, "Creator")
        };

        return GenerateToken(claims);
    }

    private (string AccessToken, DateTime ExpiresAtUtc) GenerateToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_options.ExpirationInMinutes);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return (accessToken, expiresAtUtc);
    }
}