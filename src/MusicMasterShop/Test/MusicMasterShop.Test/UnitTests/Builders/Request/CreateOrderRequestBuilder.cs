namespace MusicMasterShop.Test.UnitTests.Builders.Request;

public sealed class CreateOrderRequestBuilder
{
    private Guid _carrinhoId = Guid.NewGuid();
    private string _documentoCliente = "12345678901";

    public CreateOrderRequestBuilder WithCarrinhoId(Guid value)
    {
        _carrinhoId = value;
        return this;
    }

    public CreateOrderRequestBuilder WithDocumentoCliente(string value)
    {
        _documentoCliente = value;
        return this;
    }

    public CreateOrderRequest Build() => new(_carrinhoId, _documentoCliente);
}
