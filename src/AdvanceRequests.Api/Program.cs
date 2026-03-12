using AdvanceRequests.Api.Extensions;
using AdvanceRequests.Api.Middleware;
using AdvanceRequests.Infrastructure;
using AdvanceRequests.Api.Dispatching;
using AdvanceRequests.Application.Abstractions.Dispatching;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApiBehaviorConfiguration();
builder.Services.AddApiValidation();
builder.Services.AddApiVersioningConfiguration();
builder.Services.AddApiDocumentation();
builder.Services.AddApplicationHandlers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseApiDocumentation();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.ApplyMigrations();

app.Run();

public partial class Program
{
}