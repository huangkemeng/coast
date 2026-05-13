using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace RequirementTrackingSystem.Infrastructure.SeqLog;

public static class AddSeqLogExtensions
{
    public static void AddSeqLog(this ILoggingBuilder loggingBuilder, Action<LoggerConfiguration>? configure = null)
    {
        var serviceProvider = loggingBuilder.Services.BuildServiceProvider();
        var seqSetting = serviceProvider.GetRequiredService<SeqSetting>();
        var applicationName = "RequirementTrackingSystem";
        var loggerConfiguration = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", applicationName)
            .Enrich.WithCorrelationIdHeader()
            .Enrich.WithRequestHeader("x-tz")
            .Enrich.WithRequestHeader("authorization")
            .Enrich.WithCorrelationIdHeader()
            .Enrich.WithClientIp();
        configure?.Invoke(loggerConfiguration);
        var logger = loggerConfiguration
            .WriteTo.Console()
            .WriteTo.Seq(seqSetting.ServerUrl, apiKey: seqSetting.ApiKey)
            .CreateLogger();
        Log.Logger = logger;
        loggingBuilder.AddSerilog(Log.Logger);
        loggingBuilder.Services.AddSingleton(Log.Logger);
    }
}