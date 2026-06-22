namespace MusicMasterShop.Test.UnitTests.Builders.Request;

public sealed class CreateCartRequestBuilder
{
    private int _quantidade = 2;
    private Guid _produtoId = Guid.NewGuid();

    public CreateCartRequestBuilder WithQuantidade(int value)
    {
        _quantidade = value;
        return this;
    }

    public CreateCartRequestBuilder WithProdutoId(Guid value)
    {
        _produtoId = value;
        return this;
    }

    public CreateCartRequest Build() => new(_quantidade, _produtoId);
}
