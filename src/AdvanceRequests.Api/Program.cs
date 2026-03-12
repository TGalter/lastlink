using AdvanceRequests.Api.Extensions;
using AdvanceRequests.Api.Middleware;
using AdvanceRequests.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApiBehaviorConfiguration();
builder.Services.AddApiValidation();
builder.Services.AddApiVersioningConfiguration();
builder.Services.AddApiDocumentation();
builder.Services.AddApplicationHandlers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

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