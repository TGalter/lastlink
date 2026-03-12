using AdvanceRequests.Application.Features.AdvanceRequests.CreateAdvanceRequest;
using FluentValidation;

namespace AdvanceRequests.Api.Validators;

public sealed class CreateAdvanceRequestCommandValidator : AbstractValidator<CreateAdvanceRequestCommand>
{
    public CreateAdvanceRequestCommandValidator()
    {
        RuleFor(x => x.CreatorId)
            .NotEmpty()
            .WithMessage("CreatorId is required.");

        RuleFor(x => x.GrossAmount)
            .GreaterThan(0)
            .WithMessage("GrossAmount must be greater than zero.");
    }
}