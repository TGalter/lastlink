using AdvanceRequests.Application.Features.AdvanceRequests.GetAdvanceRequestSimulation;
using FluentValidation;

namespace AdvanceRequests.Api.Validators;

public sealed class GetAdvanceRequestSimulationQueryValidator : AbstractValidator<GetAdvanceRequestSimulationQuery>
{
    public GetAdvanceRequestSimulationQueryValidator()
    {
        RuleFor(x => x.GrossAmount)
            .GreaterThan(0)
            .WithMessage("GrossAmount must be greater than zero.");
    }
}