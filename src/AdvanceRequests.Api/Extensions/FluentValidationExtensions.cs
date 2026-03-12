using AdvanceRequests.Api.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace AdvanceRequests.Api.Extensions;

public static class FluentValidationExtensions
{
    public static IServiceCollection AddApiValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<CreateAdvanceRequestRequestValidator>();

        return services;
    }
}