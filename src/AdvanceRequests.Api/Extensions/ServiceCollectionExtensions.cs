using AdvanceRequests.Application.Features.AdvanceRequests.ApproveAdvanceRequest;
using AdvanceRequests.Application.Features.AdvanceRequests.CreateAdvanceRequest;
using AdvanceRequests.Application.Features.AdvanceRequests.GetAdvanceRequestSimulation;
using AdvanceRequests.Application.Features.AdvanceRequests.ListAdvanceRequests;
using AdvanceRequests.Application.Features.AdvanceRequests.RejectAdvanceRequest;

namespace AdvanceRequests.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationHandlers(this IServiceCollection services)
    {
        services.AddScoped<CreateAdvanceRequestHandler>();
        services.AddScoped<ApproveAdvanceRequestHandler>();
        services.AddScoped<RejectAdvanceRequestHandler>();
        services.AddScoped<GetAdvanceRequestSimulationHandler>();
        services.AddScoped<ListAdvanceRequestsHandler>();

        return services;
    }
}