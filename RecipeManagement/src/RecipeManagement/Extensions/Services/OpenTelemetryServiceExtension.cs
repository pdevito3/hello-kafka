namespace RecipeManagement.Extensions.Services;

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

public static class OpenTelemetryServiceExtension
{
    public static void OpenTelemetryRegistration(this IServiceCollection services, string serviceName)
    {
        services.AddOpenTelemetryMetrics(metrics =>
        {
            metrics
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddHttpClientInstrumentation()
                .AddEventCountersInstrumentation(c =>
                {
                    // https://learn.microsoft.com/en-us/dotnet/core/diagnostics/available-counters
                    c.AddEventSources(
                        "Microsoft.AspNetCore.Hosting",
                        // There's currently a bug preventing this from working
                        // "Microsoft-AspNetCore-Server-Kestrel"
                        "System.Net.Http", 
                        "System.Net.Sockets",
                        "System.Net.NameResolution",
                        "System.Net.Security");
                });
        });
        
        services.AddOpenTelemetryTracing(builder =>
        {
            builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(serviceName)
                    .AddTelemetrySdk()
                    .AddEnvironmentVariableDetector())
                .AddSource("MassTransit")
                .AddSource("Npgsql")
                // The following subscribes to activities from Activity Source
                // named "MyCompany.MyProduct.MyLibrary" only.
                // .AddSource("MyCompany.MyProduct.MyLibrary")
                .AddSqlClientInstrumentation(opt => opt.SetDbStatementForText = true)
                .AddAspNetCoreInstrumentation()
                .AddJaegerExporter(o =>
                {
                    o.AgentHost = Environment.GetEnvironmentVariable("JAEGER_HOST");
                    o.AgentPort = 58642;
                    o.MaxPayloadSizeInBytes = 4096;
                    o.ExportProcessorType = ExportProcessorType.Batch;
                    o.BatchExportProcessorOptions = new BatchExportProcessorOptions<System.Diagnostics.Activity>
                    {
                        MaxQueueSize = 2048,
                        ScheduledDelayMilliseconds = 5000,
                        ExporterTimeoutMilliseconds = 30000,
                        MaxExportBatchSize = 512,
                    };
                });
        });
    }
}