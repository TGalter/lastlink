using AdvanceRequests.Api.Contracts.AdvanceRequests;
using FluentValidation;

namespace AdvanceRequests.Api.Validators;

public sealed class CreateAdvanceRequestRequestValidator : AbstractValidator<CreateAdvanceRequestRequest>
{
    public CreateAdvanceRequestRequestValidator()
    {
        RuleFor(x => x.CreatorId)
            .NotEmpty()
            .WithMessage("O campo CreatorId é obrigatório.");

        RuleFor(x => x.GrossAmount)
            .GreaterThan(0)
            .WithMessage("O campo GrossAmount deve ser maior que zero.");
    }
}