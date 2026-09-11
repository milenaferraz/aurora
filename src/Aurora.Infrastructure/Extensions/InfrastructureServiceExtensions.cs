using System;
using Aurora.Application.Interfaces;
using Aurora.Infrastructure.Hermes;
using Aurora.Infrastructure.Mocks;
using Aurora.Infrastructure.Voice;
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

        var apiKey = hermesOptions.ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
            apiKey = configuration["HERMES_API_KEY"] ?? configuration["API_SERVER_KEY"] ?? string.Empty;

        hermesOptions = hermesOptions with { ApiKey = apiKey };

        if (string.IsNullOrWhiteSpace(hermesOptions.BaseUrl))
            throw new InvalidOperationException("Hermes:BaseUrl must be configured.");

        if (string.IsNullOrWhiteSpace(hermesOptions.ApiKey))
            throw new InvalidOperationException("Hermes:ApiKey must be configured (set HERMES_API_KEY or API_SERVER_KEY).");

        services.Configure<HermesOptions>(options =>
        {
            options.BaseUrl = hermesOptions.BaseUrl;
            options.ApiKey = hermesOptions.ApiKey;
            options.Model = hermesOptions.Model;
        });
        services.AddHttpClient("hermes");
        services.AddHttpClient("elevenlabs");
        services.AddSingleton<IHermesClient, HermesHttpClient>();

        services.AddSingleton<IVoiceTranscriptionService, VoiceTranscriptionService>();
        services.AddSingleton<IVoiceSpeechService, VoiceSpeechService>();

        services.AddSingleton<ICalendarProvider, MockCalendarProvider>();
        services.AddSingleton<ITaskProvider, MockTaskProvider>();
        services.AddSingleton<IEmailProvider, MockEmailProvider>();

        return services;
    }
}
