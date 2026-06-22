using Microsoft.EntityFrameworkCore;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.InfraData.Context;

namespace MusicMasterShop.InfraData.Repositories;

public sealed class CarrinhoRepository : BaseRepository<Carrinho>, ICarrinhoRepository
{
    public CarrinhoRepository(EFContext context) : base(context)
    {
    }

    public Task<Carrinho?> GetWithProductsAsync(Guid id, CancellationToken cancellationToken)
    {
        return Context.Carrinhos
            .Include(carrinho => carrinho.Produtos)
            .ThenInclude(carrinhoProduto => carrinhoProduto.Produto)
            .FirstOrDefaultAsync(carrinho => carrinho.Id == id, cancellationToken);
    }

    public Task<Carrinho?> GetActiveByUsuarioIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken)
    {
        return Context.Carrinhos
            .Include(carrinho => carrinho.Produtos)
            .ThenInclude(carrinhoProduto => carrinhoProduto.Produto)
            .FirstOrDefaultAsync(
                carrinho => carrinho.UsuarioId == usuarioId && carrinho.Ativo,
                cancellationToken);
    }
}
