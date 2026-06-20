using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.InfraData.Repositories;
using MusicMasterShop.InfraData.TransactionHandler;

namespace MusicMasterShop.WebApi.DependencyInjection
{
    internal static class InfrastructureRegister
    {
        internal static void AddInfraServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        }
    }
}
