using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace MusicMasterShop.WebApi.Configuration
{
    internal static class OpenTelemetryConfiguration
    {
        private const string serviceResourceName = "open-telemetry-music-master";
        private const string serviceResourceVersion = "1.0";
        internal static IOpenTelemetryBuilder AddOpenTelemetryConfiguration(this WebApplicationBuilder builder)
        {
            return builder.Services.AddOpenTelemetry()
                   .ConfigureResource(resource => resource.AddService(serviceResourceName))
                   .WithTracing(tracing => tracing
                      .AddSource(serviceResourceName)
                      .SetResourceBuilder(GetResourceBuilder())
                      .AddEntityFrameworkCoreInstrumentation()
                      .AddAspNetCoreInstrumentation()
                      .AddConsoleExporter()
                      .AddOtlpExporter(x => { 
                        x.Endpoint = new Uri(builder.Configuration["OtlpExporter:Endpoint"]!); 
                   }))
                   .WithMetrics(metrics => metrics
                      .AddAspNetCoreInstrumentation() 
                      .AddHttpClientInstrumentation()
                      .AddOtlpExporter());
        }

        internal static ILoggingBuilder AddOpenTelemetryLoggingConfiguration(this WebApplicationBuilder builder)
        {
            return builder.Logging.AddOpenTelemetry(options =>
                {
                    options.SetResourceBuilder(GetResourceBuilder());
                    options.IncludeFormattedMessage = true;
                    options.IncludeScopes = true;
                    options.ParseStateValues = true;
                    options.AddOtlpExporter();
                    options.AddConsoleExporter();                
                });
        }      
        
        private static ResourceBuilder GetResourceBuilder()
        {
            return ResourceBuilder.CreateDefault()
                 .AddService(serviceName: serviceResourceName,
                  serviceVersion: serviceResourceVersion);
        }
    }
}
