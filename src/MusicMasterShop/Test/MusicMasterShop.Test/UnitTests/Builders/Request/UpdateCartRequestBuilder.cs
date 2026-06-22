namespace MusicMasterShop.Test.UnitTests.Builders.Request;

public sealed class UpdateCartRequestBuilder
{
    private Guid _carrinhoId = Guid.NewGuid();
    private ICollection<UpdateCartItemRequest> _produtos =
        [new UpdateCartItemRequest(Guid.NewGuid(), 2)];

    public UpdateCartRequestBuilder WithCarrinhoId(Guid value)
    {
        _carrinhoId = value;
        return this;
    }

    public UpdateCartRequestBuilder WithProdutos(ICollection<UpdateCartItemRequest> value)
    {
        _produtos = value;
        return this;
    }

    public UpdateCartRequest Build() => new(_produtos) { CarrinhoId = _carrinhoId };
}
