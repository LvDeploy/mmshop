using Microsoft.EntityFrameworkCore;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.InfraData.Context;

namespace MusicMasterShop.InfraData.Repositories;

public sealed class PedidoRepository : BaseRepository<Pedido>, IPedidoRepository
{
    public PedidoRepository(EFContext context) : base(context)
    {
    }

    public Task<Pedido?> GetWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return Context.Pedidos
            .Include(pedido => pedido.Carrinho)
            .ThenInclude(carrinho => carrinho.Usuario)
            .Include(pedido => pedido.Carrinho)
            .ThenInclude(carrinho => carrinho.Produtos)
            .ThenInclude(carrinhoProduto => carrinhoProduto.Produto)
            .FirstOrDefaultAsync(pedido => pedido.Id == id, cancellationToken);
    }
}
