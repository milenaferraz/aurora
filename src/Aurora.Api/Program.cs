using Aurora.Api.HealthChecks;
using Aurora.Api.Middleware;
using Aurora.Api.Validators;
using Aurora.Application.Chat;
using Aurora.Application.Dashboard;
using Aurora.Infrastructure.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// CORS
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:5173" };
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()));

// Infrastructure (Hermes + mock providers)
builder.Services.AddInfrastructure(builder.Configuration);

// Application services
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<ChatStreamingService>();
builder.Services.AddScoped<DashboardService>();

// Health checks
builder.Services.AddHealthChecks()
    .AddCheck("aurora-api", () => HealthCheckResult.Healthy())
    .AddCheck<HermesHealthCheck>("hermes");

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ChatRequestValidator>();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Aurora API",
        Version = "v1"
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Aurora API v1"));
}

app.UseSerilogRequestLogging(opts =>
    opts.EnrichDiagnosticContext = (dc, ctx) =>
        dc.Set("CorrelationId", ctx.Items["CorrelationId"]));

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (ctx, report) =>
    {
        ctx.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            entries = report.Entries.ToDictionary(
                e => e.Key,
                e => new { status = e.Value.Status.ToString(), description = e.Value.Description })
        });
        await ctx.Response.WriteAsync(result);
    }
});
app.Run();

public partial class Program { }
