namespace MusicMasterShop.Test.UnitTests.Application.Queries.Handlers;

public sealed class ListProductsPagedQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithInvalidRequest_ReturnsBadRequest()
    {
        var repository = new Mock<IProdutoRepository>();
        var handler = new ListProductsPagedQueryHandler(repository.Object);
        var request = new ListProductsPagedRequestBuilder().WithPageNumber(0).Build();

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
        repository.Verify(
            x => x.GetAllPagedWithDetailsAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ReturnsMappedPage()
    {
        var repository = new Mock<IProdutoRepository>();
        Produto produto = new ProdutoBuilder().Build();
        var page = new PagedResult<Produto>
        {
            Items = [produto],
            CurrentPage = 2,
            PageSize = 5,
            TotalCount = 7
        };
        repository.Setup(x => x.GetAllPagedWithDetailsAsync(5, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(page);
        var handler = new ListProductsPagedQueryHandler(repository.Object);
        var request = new ListProductsPagedRequestBuilder()
            .WithPageNumber(2)
            .WithPageSize(5)
            .Build();

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data!.Items);
        Assert.Equal(2, result.Data.CurrentPage);
        Assert.Equal(produto.Id, result.Data.Items[0].Id);
    }
}
