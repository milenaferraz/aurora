using System;
using Aurora.Application.Interfaces;
using Aurora.Infrastructure.Hermes;
using Aurora.Infrastructure.Mocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aurora.Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var hermesOptions = configuration.GetSection("Hermes").Get<HermesOptions>()
            ?? new HermesOptions();

        if (!hermesOptions.UseFake && string.IsNullOrWhiteSpace(hermesOptions.BaseUrl))
            throw new InvalidOperationException(
                "Hermes:BaseUrl must be configured when Hermes:UseFake is false.");

        services.Configure<HermesOptions>(configuration.GetSection("Hermes"));
        services.AddHttpClient("hermes");

        services.AddSingleton<IHermesClient, FakeHermesClient>();

        services.AddSingleton<ICalendarProvider, MockCalendarProvider>();
        services.AddSingleton<ITaskProvider, MockTaskProvider>();
        services.AddSingleton<IEmailProvider, MockEmailProvider>();

        return services;
    }
}
