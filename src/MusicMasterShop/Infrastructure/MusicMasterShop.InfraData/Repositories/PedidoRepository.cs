using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.InfraData.Context;

namespace MusicMasterShop.InfraData.Repositories;

public sealed class PedidoRepository : BaseRepository<Pedido>, IPedidoRepository
{
    public PedidoRepository(EFContext context) : base(context)
    {
    }
}
