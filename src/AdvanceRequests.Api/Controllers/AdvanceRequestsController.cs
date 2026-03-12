using AdvanceRequests.Api.Contracts.AdvanceRequests;
using AdvanceRequests.Application.Features.AdvanceRequests.ApproveAdvanceRequest;
using AdvanceRequests.Application.Features.AdvanceRequests.CreateAdvanceRequest;
using AdvanceRequests.Application.Features.AdvanceRequests.GetAdvanceRequestSimulation;
using AdvanceRequests.Application.Features.AdvanceRequests.ListAdvanceRequests;
using AdvanceRequests.Application.Features.AdvanceRequests.RejectAdvanceRequest;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using AdvanceRequests.Application.Abstractions.Services;
using AdvanceRequests.Domain.Enums;

namespace AdvanceRequests.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/advance-requests")]
[Authorize]
public sealed class AdvanceRequestsController : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Creator")]
    public async Task<IActionResult> Create(
    [FromBody] CreateAdvanceRequestRequest request,
    [FromServices] CreateAdvanceRequestHandler handler,
    [FromServices] ICurrentUserService currentUser,
    CancellationToken cancellationToken)
    {
        if (!currentUser.CreatorId.HasValue)
        {
            return Unauthorized(new { erro = "Token do creator inválido." });
        }

        var command = new CreateAdvanceRequestCommand
        {
            CreatorId = currentUser.CreatorId.Value,
            GrossAmount = request.GrossAmount
        };

        var result = await handler.Handle(command, cancellationToken);

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> List(
    [FromQuery] AdvanceRequestStatus? status,
    [FromQuery] DateTime? fromDate,
    [FromQuery] DateTime? toDate,
    [FromQuery] Guid? creatorId,
    [FromServices] ListAdvanceRequestsHandler handler,
    [FromServices] ICurrentUserService currentUser,
    CancellationToken cancellationToken)
    {
        var query = new ListAdvanceRequestsQuery
        {
            Status = status,
            FromDate = fromDate,
            ToDate = toDate,
            CreatorId = creatorId,
            IsAdmin = string.Equals(currentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase),
            RequestingCreatorId = currentUser.CreatorId
        };

        var result = await handler.Handle(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { erro = result.Error });

        return Ok(result.Value);
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(
        Guid id,
        [FromServices] ApproveAdvanceRequestHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new ApproveAdvanceRequestCommand
        {
            AdvanceRequestId = id
        };

        await handler.Handle(command, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reject(
        Guid id,
        [FromServices] RejectAdvanceRequestHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new RejectAdvanceRequestCommand
        {
            AdvanceRequestId = id
        };

        await handler.Handle(command, cancellationToken);

        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("simulate")]
    public IActionResult Simulate(
            [FromQuery] SimulateAdvanceRequestRequest request,
            [FromServices] GetAdvanceRequestSimulationHandler handler)
    {
        var result = handler.Handle(new GetAdvanceRequestSimulationQuery
        {
            GrossAmount = request.Amount
        });

        return Ok(result.Value);
    }
}