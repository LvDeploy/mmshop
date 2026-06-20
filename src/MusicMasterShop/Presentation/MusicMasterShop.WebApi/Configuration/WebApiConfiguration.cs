using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace MusicMasterShop.WebApi.Configuration
{
    [ExcludeFromCodeCoverage]
    internal static class WebApiConfiguration
    {
        internal static void AddWebApiConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
            builder.Services.AddOutputCache(options =>
            {
                options.AddBasePolicy(builder => builder.NoCache());
            });
            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                }).AddDataAnnotationsLocalization();

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1.0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options =>
            {
               options.GroupNameFormat = "'v'VVV";
               options.SubstituteApiVersionInUrl = true;
            });
        }

        internal static void UseWebApplicationConfiguration(this WebApplication app)
        {
            app.UseHttpsRedirection();
            app.UseCorsConfiguration();
        }
    }
}
