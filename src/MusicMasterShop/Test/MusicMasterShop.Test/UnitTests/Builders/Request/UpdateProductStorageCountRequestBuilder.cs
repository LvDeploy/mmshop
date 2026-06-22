namespace MusicMasterShop.Test.UnitTests.Builders.Request;

public sealed class UpdateProductStorageCountRequestBuilder
{
    private Guid _id = Guid.NewGuid();
    private int _quantidade = 10;
    private string _numeroNotaFiscal = "NF-001";

    public UpdateProductStorageCountRequestBuilder WithId(Guid value)
    {
        _id = value;
        return this;
    }

    public UpdateProductStorageCountRequestBuilder WithQuantidade(int value)
    {
        _quantidade = value;
        return this;
    }

    public UpdateProductStorageCountRequestBuilder WithNumeroNotaFiscal(string value)
    {
        _numeroNotaFiscal = value;
        return this;
    }

    public UpdateProductStorageCountRequest Build() =>
        new(_quantidade, _numeroNotaFiscal) { Id = _id };
}
