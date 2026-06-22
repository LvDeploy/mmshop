using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.Domain.Enums;
using MusicMasterShop.InfraData.Context;

namespace MusicMasterShop.InfraData.Repositories
{
    public class CategoriaRepository : BaseRepository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(EFContext context) : base(context)
        {
        }
        public Task<Categoria?> GetByTipoAsync(TipoCategoria tipo, CancellationToken cancellationToken)
        {
            return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
                Context.Categorias,
                cat => cat.Tipo == tipo,
                cancellationToken);
        }
    }
}
