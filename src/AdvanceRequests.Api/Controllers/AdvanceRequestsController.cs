using AdvanceRequests.Api.Contracts.AdvanceRequests;
using AdvanceRequests.Application.Features.AdvanceRequests.ApproveAdvanceRequest;
using AdvanceRequests.Application.Features.AdvanceRequests.CreateAdvanceRequest;
using AdvanceRequests.Application.Features.AdvanceRequests.GetAdvanceRequestSimulation;
using AdvanceRequests.Application.Features.AdvanceRequests.ListAdvanceRequests;
using AdvanceRequests.Application.Features.AdvanceRequests.RejectAdvanceRequest;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceRequests.Api.Controllers;

[ApiController]
[Route("api/v1/advance-requests")]
public sealed class AdvanceRequestsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateAdvanceRequestRequest request,
        [FromServices] CreateAdvanceRequestHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateAdvanceRequestCommand
        {
            CreatorId = request.CreatorId,
            GrossAmount = request.GrossAmount
        };

        var result = await handler.Handle(command, cancellationToken);

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromServices] ListAdvanceRequestsHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new ListAdvanceRequestsQuery(), cancellationToken);

        return Ok(result.Value);
    }

    [HttpPost("{id}/approve")]
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