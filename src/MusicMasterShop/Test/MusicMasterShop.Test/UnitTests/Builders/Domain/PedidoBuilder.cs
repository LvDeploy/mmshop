namespace MusicMasterShop.Test.UnitTests.Builders.Domain;

public sealed class PedidoBuilder
{
    private string _documento = "12345678901";
    private Carrinho _carrinho = new CarrinhoBuilder().Build();
    private StatusPedido _status = StatusPedido.Validado;

    public PedidoBuilder WithCarrinho(Carrinho value)
    {
        _carrinho = value;
        return this;
    }

    public Pedido Build() => Pedido.Create(_documento, _carrinho, _status);
}
