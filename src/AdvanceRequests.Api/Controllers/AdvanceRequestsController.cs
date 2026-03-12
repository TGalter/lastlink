using Asp.Versioning;
using AdvanceRequests.Api.Contracts.AdvanceRequests;
using AdvanceRequests.Application.Abstractions.Dispatching;
using AdvanceRequests.Application.Abstractions.Services;
using AdvanceRequests.Application.DTOs;
using AdvanceRequests.Application.Features.AdvanceRequests.ApproveAdvanceRequest;
using AdvanceRequests.Application.Features.AdvanceRequests.CreateAdvanceRequest;
using AdvanceRequests.Application.Features.AdvanceRequests.GetAdvanceRequestSimulation;
using AdvanceRequests.Application.Features.AdvanceRequests.ListAdvanceRequests;
using AdvanceRequests.Application.Features.AdvanceRequests.RejectAdvanceRequest;
using AdvanceRequests.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceRequests.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/advance-requests")]
[Authorize]
public sealed class AdvanceRequestsController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICurrentUserService _currentUser;

    public AdvanceRequestsController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ICurrentUserService currentUser)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _currentUser = currentUser;
    }

    [HttpPost]
    [Authorize(Roles = "Creator")]
    public async Task<IActionResult> Create(
        [FromBody] CreateAdvanceRequestRequest request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.CreatorId.HasValue)
            return Unauthorized(new { erro = "Token do creator inválido." });

        var command = new CreateAdvanceRequestCommand
        {
            CreatorId = _currentUser.CreatorId.Value,
            GrossAmount = request.GrossAmount
        };

        var result = await _commandDispatcher.DispatchAsync<CreateAdvanceRequestCommand, AdvanceRequestDto>(
            command,
            cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { erro = result.Error });

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] AdvanceRequestStatus? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] Guid? creatorId,
        CancellationToken cancellationToken)
    {
        var query = new ListAdvanceRequestsQuery
        {
            Status = status,
            FromDate = fromDate,
            ToDate = toDate,
            CreatorId = creatorId,
            IsAdmin = string.Equals(_currentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase),
            RequestingCreatorId = _currentUser.CreatorId
        };

        var result = await _queryDispatcher.DispatchAsync<ListAdvanceRequestsQuery, IReadOnlyList<AdvanceRequestDto>>(
            query,
            cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { erro = result.Error });

        return Ok(result.Value);
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var command = new ApproveAdvanceRequestCommand
        {
            AdvanceRequestId = id
        };

        var result = await _commandDispatcher.DispatchAsync(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { erro = result.Error });

        return NoContent();
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reject(Guid id, CancellationToken cancellationToken)
    {
        var command = new RejectAdvanceRequestCommand
        {
            AdvanceRequestId = id
        };

        var result = await _commandDispatcher.DispatchAsync(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { erro = result.Error });

        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("simulate")]
    public async Task<IActionResult> Simulate(
        [FromQuery] SimulateAdvanceRequestRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetAdvanceRequestSimulationQuery
        {
            GrossAmount = request.Amount
        };

        var result = await _queryDispatcher.DispatchAsync<GetAdvanceRequestSimulationQuery, AdvanceRequestSimulationDto>(
            query,
            cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { erro = result.Error });

        return Ok(result.Value);
    }
}