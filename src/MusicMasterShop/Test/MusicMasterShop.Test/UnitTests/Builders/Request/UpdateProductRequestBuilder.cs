namespace MusicMasterShop.Test.UnitTests.Builders.Request;

public sealed class UpdateProductRequestBuilder
{
    private Guid _id = Guid.NewGuid();
    private string? _nome = "Guitarra atualizada";
    private string? _descricao = "Descrição atualizada";
    private decimal _preco = 2099.90m;
    private TipoCategoria? _tipoCategoria = TipoCategoria.Corda;

    public UpdateProductRequestBuilder WithId(Guid value)
    {
        _id = value;
        return this;
    }

    public UpdateProductRequestBuilder WithNome(string? value)
    {
        _nome = value;
        return this;
    }

    public UpdateProductRequestBuilder WithDescricao(string? value)
    {
        _descricao = value;
        return this;
    }

    public UpdateProductRequestBuilder WithPreco(decimal value)
    {
        _preco = value;
        return this;
    }

    public UpdateProductRequestBuilder WithTipoCategoria(TipoCategoria? value)
    {
        _tipoCategoria = value;
        return this;
    }

    public UpdateProductRequest Build() =>
        new(_nome, _descricao, _preco, _tipoCategoria) { Id = _id };
}
