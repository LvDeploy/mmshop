using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MusicMasterShop.WebApi.Configuration;

internal static class AuthenticationConfiguration
{
    internal static void AddAuthenticationConfiguration(this WebApplicationBuilder builder)
    {
        string key = builder.Configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("'Jwt:Key' nao existe.");
        string issuer = builder.Configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("'Jwt:Issuer' nao existe.");
        string audience = builder.Configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("'Jwt:Audience' nao existe.");

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddAuthorization();
    }

    internal static void UseAuthenticationConfiguration(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}
