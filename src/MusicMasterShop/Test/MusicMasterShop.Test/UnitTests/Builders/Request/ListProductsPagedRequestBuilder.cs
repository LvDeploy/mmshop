namespace MusicMasterShop.Test.UnitTests.Builders.Request;

public sealed class ListProductsPagedRequestBuilder
{
    private int _pageNumber = 1;
    private int _pageSize = 10;

    public ListProductsPagedRequestBuilder WithPageNumber(int value)
    {
        _pageNumber = value;
        return this;
    }

    public ListProductsPagedRequestBuilder WithPageSize(int value)
    {
        _pageSize = value;
        return this;
    }

    public ListProductsPagedRequest Build() => new(_pageNumber, _pageSize);
}
