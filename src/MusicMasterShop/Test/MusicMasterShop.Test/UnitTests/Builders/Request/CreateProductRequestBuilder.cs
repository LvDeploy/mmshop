namespace MusicMasterShop.Test.UnitTests.Builders.Request;

public sealed class CreateProductRequestBuilder
{
    private string _nome = "Guitarra";
    private string _descricao = "Guitarra elétrica";
    private decimal _preco = 1999.90m;
    private TipoCategoria? _tipoCategoria = TipoCategoria.Corda;

    public CreateProductRequestBuilder WithNome(string value)
    {
        _nome = value;
        return this;
    }

    public CreateProductRequestBuilder WithDescricao(string value)
    {
        _descricao = value;
        return this;
    }

    public CreateProductRequestBuilder WithPreco(decimal value)
    {
        _preco = value;
        return this;
    }

    public CreateProductRequestBuilder WithTipoCategoria(TipoCategoria? value)
    {
        _tipoCategoria = value;
        return this;
    }

    public CreateProductRequest Build() => new(_nome, _descricao, _preco, _tipoCategoria);
}
