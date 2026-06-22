using MusicMasterShop.Domain.Contracts.Repositories.Base;
using MusicMasterShop.Domain.Entities;

namespace MusicMasterShop.Domain.Contracts.Repositories;

public interface ICarrinhoRepository : IBaseRepository<Carrinho>
{
    Task<Carrinho?> GetWithProductsAsync(Guid id, CancellationToken cancellationToken);
    Task<Carrinho?> GetActiveByUsuarioIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken);
}
