using AdvanceRequests.Api.Contracts.AdvanceRequests;
using FluentValidation;

namespace AdvanceRequests.Api.Validators;

public sealed class SimulateAdvanceRequestRequestValidator : AbstractValidator<SimulateAdvanceRequestRequest>
{
    public SimulateAdvanceRequestRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.");
    }
}