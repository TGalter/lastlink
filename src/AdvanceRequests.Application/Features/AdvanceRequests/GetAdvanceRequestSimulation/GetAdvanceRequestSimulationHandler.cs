using AdvanceRequests.Application.Common;
using AdvanceRequests.Application.DTOs;
using AdvanceRequests.Domain.Exceptions;

namespace AdvanceRequests.Application.Features.AdvanceRequests.GetAdvanceRequestSimulation;

public sealed class GetAdvanceRequestSimulationHandler
{
    private const decimal FeePercentage = 0.05m;

    public Result<AdvanceRequestSimulationDto> Handle(GetAdvanceRequestSimulationQuery query)
    {
        if (query.GrossAmount <= 100)
            throw new InvalidAdvanceRequestAmountException(query.GrossAmount);

        var feeAmount = Math.Round(query.GrossAmount * FeePercentage, 2, MidpointRounding.ToEven);
        var netAmount = query.GrossAmount - feeAmount;

        return Result<AdvanceRequestSimulationDto>.Success(new AdvanceRequestSimulationDto
        {
            GrossAmount = query.GrossAmount,
            FeePercentage = FeePercentage,
            FeeAmount = feeAmount,
            NetAmount = netAmount
        });
    }
}