using Microsoft.EntityFrameworkCore;
using MusicMasterShop.InfraData.Context;

namespace MusicMasterShop.WebApi.Configuration
{
    public static class EFContextConfiguration
    {
        public static void AddEFContextConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<EFContext>(opt => opt.UseInMemoryDatabase("test"));
        }
        
        public static void CreateDataBase(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dataContext = scope.ServiceProvider.GetService<EFContext>();
            dataContext?.Database.EnsureCreated();
        }
    }
}
