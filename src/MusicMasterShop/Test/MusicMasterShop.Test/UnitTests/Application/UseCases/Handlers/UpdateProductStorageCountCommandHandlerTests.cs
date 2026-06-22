namespace MusicMasterShop.Test.UnitTests.Application.UseCases.Handlers;

public sealed class UpdateProductStorageCountCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IProdutoRepository> _repository = new();

    [Fact]
    public async Task Handle_WithInvalidRequest_ReturnsBadRequest()
    {
        var handler = new UpdateProductStorageCountCommandHandler(
            _unitOfWork.Object,
            _repository.Object);

        var result = await handler.Handle(
            new UpdateProductStorageCountRequestBuilder().WithId(Guid.Empty).Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsNotFound()
    {
        _repository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Produto)null!);
        var handler = new UpdateProductStorageCountCommandHandler(
            _unitOfWork.Object,
            _repository.Object);

        var result = await handler.Handle(
            new UpdateProductStorageCountRequestBuilder().Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenProductExists_UpdatesStorageAndCommits()
    {
        Produto produto = new ProdutoBuilder().WithQuantidade(1).Build();
        _repository.Setup(x => x.Get(produto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);
        var handler = new UpdateProductStorageCountCommandHandler(
            _unitOfWork.Object,
            _repository.Object);

        var result = await handler.Handle(
            new UpdateProductStorageCountRequestBuilder()
                .WithId(produto.Id)
                .WithQuantidade(25)
                .WithNumeroNotaFiscal("NF-999")
                .Build(),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal((uint)25, produto.QtdDisponivel);
        Assert.Equal("NF-999", produto.NumeroNotaFiscal);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
