using AdvanceRequests.Api.Contracts.AdvanceRequests;
using FluentValidation;

namespace AdvanceRequests.Api.Validators;

public sealed class CreateAdvanceRequestRequestValidator : AbstractValidator<CreateAdvanceRequestRequest>
{
    public CreateAdvanceRequestRequestValidator()
    {
        RuleFor(x => x.CreatorId)
            .NotEmpty()
            .WithMessage("CreatorId is required.");

        RuleFor(x => x.GrossAmount)
            .GreaterThan(0)
            .WithMessage("GrossAmount must be greater than zero.");
    }
}