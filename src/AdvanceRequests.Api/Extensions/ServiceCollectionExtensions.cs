using AdvanceRequests.Application.Abstractions.Dispatching;
using AdvanceRequests.Application.Features.AdvanceRequests.ApproveAdvanceRequest;
using AdvanceRequests.Application.Features.AdvanceRequests.CreateAdvanceRequest;
using AdvanceRequests.Application.Features.AdvanceRequests.GetAdvanceRequestSimulation;
using AdvanceRequests.Application.Features.AdvanceRequests.ListAdvanceRequests;
using AdvanceRequests.Application.Features.AdvanceRequests.RejectAdvanceRequest;
using AdvanceRequests.Application.DTOs;

namespace AdvanceRequests.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateAdvanceRequestCommand, AdvanceRequestDto>, CreateAdvanceRequestHandler>();
        services.AddScoped<ICommandHandler<ApproveAdvanceRequestCommand>, ApproveAdvanceRequestHandler>();
        services.AddScoped<ICommandHandler<RejectAdvanceRequestCommand>, RejectAdvanceRequestHandler>();
        services.AddScoped<IQueryHandler<GetAdvanceRequestSimulationQuery, AdvanceRequestSimulationDto>, GetAdvanceRequestSimulationHandler>();
        services.AddScoped<IQueryHandler<ListAdvanceRequestsQuery, IReadOnlyList<AdvanceRequestDto>>, ListAdvanceRequestsHandler>();

        return services;
    }
}