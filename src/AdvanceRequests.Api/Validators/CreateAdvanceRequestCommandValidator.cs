using AdvanceRequests.Application.Features.AdvanceRequests.CreateAdvanceRequest;
using FluentValidation;

namespace AdvanceRequests.Api.Validators;

public sealed class CreateAdvanceRequestCommandValidator : AbstractValidator<CreateAdvanceRequestCommand>
{
    public CreateAdvanceRequestCommandValidator()
    {
        RuleFor(x => x.CreatorId)
            .NotEmpty()
            .WithMessage("O identificador do criador é obrigatório.");

        RuleFor(x => x.GrossAmount)
            .GreaterThan(100)
            .WithMessage("O valor bruto deve ser maior que 100.");
    }
}