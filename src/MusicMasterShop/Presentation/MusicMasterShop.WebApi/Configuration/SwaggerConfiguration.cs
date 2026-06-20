using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using MusicMasterShop.WebApi.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace MusicMasterShop.WebApi.Configuration
{
    [ExcludeFromCodeCoverage]
    internal static class SwaggerConfiguration
    {
        internal static void AddSwaggerConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            builder.Services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerDefaultValues>();

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            });
        }

        internal static void UseSwaggerConfiguration(this IApplicationBuilder app, IWebHostEnvironment env, IReadOnlyList<ApiVersionDescription> descriptions)
        {
            if (env != null && env.EnvironmentName == "Production")
                return;

            app.UseSwagger(options =>
            {
                options.RouteTemplate = $"{ApiInfo.BaseUrl}/swagger/{{documentName}}/swagger.json";
            });
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = $"{ApiInfo.BaseUrl}/swagger";
                foreach (var description in descriptions.OrderByDescending(p => p.GroupName))
                {
                    string url = $"/{ApiInfo.BaseUrl}/swagger/{description.GroupName}/swagger.json";
                    string name = description.GroupName.ToUpperInvariant();
                    options.SwaggerEndpoint(url, name);
                }
            });
        }
    }
}
