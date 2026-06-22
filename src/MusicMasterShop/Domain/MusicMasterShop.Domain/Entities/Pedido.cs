using MusicMasterShop.Domain.Entities.Base;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Domain.Entities
{
    public sealed class Pedido : BaseEntity
    {
        public Pedido(Guid id, string documentoCliente, StatusPedido status)
        {
            Id = id;
            DocumentoCliente = documentoCliente;
            Status = status;
        }

        public Guid CarrinhoId { get; private set; }
        public Carrinho Carrinho { get; private set; } = null!;
        public string DocumentoCliente { get; private set; } = string.Empty;
        public StatusPedido Status { get; private set; }
        public static Pedido Create(string documentoCliente, Carrinho carrinho, StatusPedido status)
        {
            var pedido = new Pedido(Guid.NewGuid(), documentoCliente, status);
            pedido.SetCreateDate(DateTime.Now);
            pedido.SetNavigationProperties(carrinho);
            return pedido;
        }
        public void SetNavigationProperties(Carrinho carrinho)
        {
            if (carrinho != null)
            {
                Carrinho = carrinho;
                CarrinhoId = carrinho.Id;
            }
        }
    }
}
