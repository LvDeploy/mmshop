using MusicMasterShop.Domain.ValueObjects;

namespace MusicMasterShop.Test.UnitTests.Builders.Domain;

public sealed class CarrinhoBuilder
{
    private Usuario _usuario = new UsuarioBuilder().Build();
    private List<(Produto Produto, int Quantidade)> _produtos =
        [(new ProdutoBuilder().Build(), 2)];
    private bool _ativo = true;

    public CarrinhoBuilder WithUsuario(Usuario value)
    {
        _usuario = value;
        return this;
    }

    public CarrinhoBuilder WithProdutos(params (Produto Produto, int Quantidade)[] values)
    {
        _produtos = [.. values];
        return this;
    }

    public CarrinhoBuilder Inativo()
    {
        _ativo = false;
        return this;
    }

    public Carrinho Build()
    {
        Produto seed = _produtos.Count > 0 ? _produtos[0].Produto : new ProdutoBuilder().Build();
        int quantidade = _produtos.Count > 0 ? _produtos[0].Quantidade : 1;
        Carrinho carrinho = Carrinho.Create(quantidade, seed, _usuario);

        ICollection<CarrinhoProduto> items = _produtos
            .Select(item => CarrinhoProduto.Create(item.Quantidade, item.Produto))
            .ToList();
        carrinho.SetNavigationProperties(items, _usuario);

        if (!_ativo)
            carrinho.Finalizar();

        return carrinho;
    }
}
