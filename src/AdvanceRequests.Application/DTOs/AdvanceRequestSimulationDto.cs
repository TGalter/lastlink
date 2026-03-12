namespace AdvanceRequests.Application.DTOs;

public sealed class AdvanceRequestSimulationDto
{
    public decimal GrossAmount { get; init; }
    public decimal FeePercentage { get; init; }
    public decimal FeeAmount { get; init; }
    public decimal NetAmount { get; init; }
}