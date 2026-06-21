using MusicMasterShop.Domain.Contracts.Repositories.Base;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Domain.Contracts.Repositories
{
    public interface ICategoriaRepository : IBaseRepository<Categoria>
    {
        Task<Categoria?> GetByTipoAsync(TipoCategoria tipo, CancellationToken cancellationToken);
    }
}
