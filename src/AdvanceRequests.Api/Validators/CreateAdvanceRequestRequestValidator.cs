using AdvanceRequests.Api.Contracts.AdvanceRequests;
using AdvanceRequests.Application.Features.AdvanceRequests.CreateAdvanceRequest;
using FluentValidation;

namespace AdvanceRequests.Api.Validators;

public sealed class CreateAdvanceRequestRequestValidator : AbstractValidator<CreateAdvanceRequestRequest>
{
    public CreateAdvanceRequestRequestValidator()
    {
        RuleFor(x => x.GrossAmount)
        .GreaterThan(0)
        .LessThanOrEqualTo(1_000_000)
        .WithMessage("O valor bruto deve ser maior que zero e menor ou igual a 1.000.000.");
    }
}