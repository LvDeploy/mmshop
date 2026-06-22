namespace MusicMasterShop.Test.UnitTests.Builders.Domain;

public sealed class ProdutoBuilder
{
    private string _nome = "Guitarra";
    private string _descricao = "Guitarra elétrica";
    private decimal _preco = 1999.90m;
    private uint _quantidade = 10;
    private string _notaFiscal = "NF-001";
    private Categoria _categoria = new CategoriaBuilder().Build();

    public ProdutoBuilder WithNome(string value)
    {
        _nome = value;
        return this;
    }

    public ProdutoBuilder WithPreco(decimal value)
    {
        _preco = value;
        return this;
    }

    public ProdutoBuilder WithQuantidade(uint value)
    {
        _quantidade = value;
        return this;
    }

    public ProdutoBuilder WithCategoria(Categoria value)
    {
        _categoria = value;
        return this;
    }

    public Produto Build()
    {
        Produto produto = Produto.Create(_nome, _descricao, _preco, _categoria);
        produto.AddQtdDisponivel(_quantidade);
        produto.AddNotaFiscal(_notaFiscal);
        return produto;
    }
}
