using AdvanceRequests.Application.Abstractions.Dispatching;
using AdvanceRequests.Application.DTOs;

namespace AdvanceRequests.Application.Features.AdvanceRequests.GetAdvanceRequestSimulation;

public sealed class GetAdvanceRequestSimulationQuery : IQuery<AdvanceRequestSimulationDto>
{
    public decimal GrossAmount { get; init; }
}