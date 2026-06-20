using FluentValidation;
using MusicMasterShop.Application.Middleware.Correlation;
using System.Reflection;

namespace MusicMasterShop.WebApi.DependencyInjection
{
    internal static class ApplicationRegister
    {
        internal static void AddAppServices(this WebApplicationBuilder builder) 
        {
            Assembly assemblyApplication = AppDomain.CurrentDomain.GetAssemblies()
                 .FirstOrDefault(a => a.GetName().Name.Equals("MusicMasterShop.Application", StringComparison.OrdinalIgnoreCase))!;

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assemblyApplication));
            builder.Services.AddValidatorsFromAssembly(assemblyApplication);
            builder.Services.AddScoped<CorrelationId>();
        }
    }
}
