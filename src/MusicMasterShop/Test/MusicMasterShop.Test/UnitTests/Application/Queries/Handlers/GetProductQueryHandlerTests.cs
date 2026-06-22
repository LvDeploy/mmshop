namespace MusicMasterShop.Test.UnitTests.Application.Queries.Handlers;

public sealed class GetProductQueryHandlerTests
{
    private readonly Mock<IProdutoRepository> _repository = new();

    [Fact]
    public async Task Handle_WhenProductExists_ReturnsMappedProduct()
    {
        Produto produto = new ProdutoBuilder().Build();
        _repository.Setup(x => x.GetWithDetailsAsync(produto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);
        var handler = new GetProductQueryHandler(_repository.Object);

        var result = await handler.Handle(
            new GetProductRequest(produto.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(produto.Id, result.Data!.Id);
        Assert.Equal(produto.Categoria.Tipo, result.Data.Categoria);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsNotFound()
    {
        _repository.Setup(x => x.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Produto)null!);
        var handler = new GetProductQueryHandler(_repository.Object);

        var result = await handler.Handle(
            new GetProductRequest(Guid.NewGuid()),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }
}
