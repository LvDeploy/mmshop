using System.Diagnostics.CodeAnalysis;

namespace MusicMasterShop.WebApi.Configuration
{
    [ExcludeFromCodeCoverage]
    internal static class CorsConfiguration
    {
        public static readonly string CorsName = "CORSPOLICY";

        internal static IServiceCollection AddCorsConfiguration(this WebApplicationBuilder builder)
        {
            string origins = builder.Configuration["Cors:Authorize"]!;

            return builder.Services.AddCors(options =>
            {
                options.AddPolicy(CorsName, config => config.WithOrigins(origins.Split(';'))
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials()
                       .WithExposedHeaders(["X-Pagination", "X-Summary", "Content-Disposition"]));
            });
        }

        internal static IApplicationBuilder UseCorsConfiguration(this IApplicationBuilder app)
        {
            return app.UseCors(CorsName);
        }
    }
}
