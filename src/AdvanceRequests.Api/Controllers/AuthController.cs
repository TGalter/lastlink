using AdvanceRequests.Api.Auth;
using AdvanceRequests.Api.Contracts.Auth;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceRequests.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login(
        [FromBody] LoginRequest request,
        [FromServices] ITokenService tokenService)
    {
        if (string.Equals(request.Role, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            var token = tokenService.GenerateAdminToken();

            return Ok(new LoginResponse
            {
                AccessToken = token.AccessToken,
                ExpiresAtUtc = token.ExpiresAtUtc
            });
        }

        if (string.Equals(request.Role, "Creator", StringComparison.OrdinalIgnoreCase))
        {
            if (!request.CreatorId.HasValue || request.CreatorId == Guid.Empty)
            {
                return BadRequest(new
                {
                    erro = "Para o perfil Creator, o campo CreatorId é obrigatório."
                });
            }

            var token = tokenService.GenerateCreatorToken(request.CreatorId.Value);

            return Ok(new LoginResponse
            {
                AccessToken = token.AccessToken,
                ExpiresAtUtc = token.ExpiresAtUtc
            });
        }

        return BadRequest(new
        {
            erro = "Perfil inválido. Use Admin ou Creator."
        });
    }
}