using System.Threading;
using System.Threading.Tasks;
using Aurora.Application.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Aurora.Api.HealthChecks;

public class HermesHealthCheck : IHealthCheck
{
    private readonly IHermesClient _hermes;

    public HermesHealthCheck(IHermesClient hermes)
    {
        _hermes = hermes;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var healthy = await _hermes.IsHealthyAsync(cancellationToken);
        return healthy
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Hermes is unreachable");
    }
}
