using System;
using Aurora.Application.Interfaces;
using Aurora.Infrastructure.Knowledge;
using Aurora.Domain.Knowledge;
using Aurora.Contracts.Knowledge;
using Aurora.Application.Knowledge;
using Aurora.Infrastructure.Hermes;
using Aurora.Infrastructure.Mocks;
using Aurora.Infrastructure.Email;
using Aurora.Infrastructure.Calendar;
using Aurora.Infrastructure.Memory;
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

        services.AddSingleton<ICalendarProvider, CalendarProvider>();
        services.AddSingleton<IMemoryProvider, MemoryProvider>();
        services.AddSingleton<ITaskProvider, MockTaskProvider>();
        services.AddSingleton<IEmailProvider, EmailProvider>();

        
    services.Configure<VaultOptions>(configuration.GetSection(VaultOptions.SectionName));
    services.AddSingleton<IMemoryService, KnowledgeService>();
    services.AddSingleton<IVaultRepository, VaultRepository>();
    services.AddSingleton<IMarkdownGenerator, MarkdownGenerator>();
    services.AddSingleton<ISlugGenerator, SlugGenerator>();

        // Register orchestrator services
        services.AddSingleton<IOrchestrator, OrchestratorService>();
        services.AddSingleton<ITaskDecomposer, DefaultTaskDecomposer>();
        services.AddSingleton<IAgentSelector, DefaultAgentSelector>();
        services.AddSingleton<IResultAggregator, DefaultResultAggregator>();
        services.AddOptions<OrchestratorOptions>()
                .Configure<IConfiguration>((settings, config) => {
                    config.GetSection("Orchestrator").Bind(settings);
                });

        // Register agent services
        services.AddTransient<DevOpsAgent>();
        services.AddTransient<SecurityAgent>();
        services.AddTransient<QAAgent>();
        services.AddSingleton<IAgentFactory, AgentFactory>();

        return services;
    }
}
