using Microsoft.EntityFrameworkCore;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.Domain.Enums;
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

            if (!dataContext!.Categorias.Any())
            {
                dataContext.Categorias.AddRange(Categoria.Create(TipoCategoria.Sopro, "Instrumento de Sopro"),
                    Categoria.Create(TipoCategoria.Corda, "Instrumento de Corda"),
                    Categoria.Create(TipoCategoria.Percussao, "Instrumento de Percussão"),
                    Categoria.Create(TipoCategoria.Tecla, "Instrumento de Tecla"));
                dataContext.SaveChanges();
            }
        }
    }
}
