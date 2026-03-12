using Microsoft.AspNetCore.Mvc;

namespace AdvanceRequests.Api.Extensions;

public static class ProblemDetailsExtensions
{
    public static IServiceCollection AddApiBehaviorConfiguration(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(error => new
                    {
                        campo = x.Key,
                        mensagem = error.ErrorMessage
                    }))
                    .ToList();

                return new BadRequestObjectResult(new
                {
                    erro = "Falha na validação",
                    detalhes = errors
                });
            };
        });

        return services;
    }
}