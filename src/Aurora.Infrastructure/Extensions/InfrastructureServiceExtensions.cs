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
        var useFake = configuration.GetValue("Hermes:UseFake", hermesOptions.UseFake);

        services.Configure<HermesOptions>(configuration.GetSection("Hermes"));
        services.AddHttpClient("hermes");
        services.AddHttpClient("elevenlabs");

        if (useFake || string.IsNullOrWhiteSpace(hermesOptions.BaseUrl))
        {
            services.AddSingleton<IHermesClient, FakeHermesClient>();
        }
        else
        {
            services.AddSingleton<IHermesClient, HermesHttpClient>();
        }

        services.AddSingleton<IVoiceTranscriptionService, VoiceTranscriptionService>();
        services.AddSingleton<IVoiceSpeechService, VoiceSpeechService>();

        services.AddSingleton<ICalendarProvider, MockCalendarProvider>();
        services.AddSingleton<ITaskProvider, MockTaskProvider>();
        services.AddSingleton<IEmailProvider, MockEmailProvider>();

        return services;
    }
}
