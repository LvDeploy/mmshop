namespace MusicMasterShop.Test.UnitTests.Application.UseCases.Handlers;

public sealed class DeleteProductCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IProdutoRepository> _repository = new();

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsNotFound()
    {
        _repository.Setup(x => x.GetWithDetailsAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Produto)null!);
        var handler = new DeleteProductCommandHandler(_unitOfWork.Object, _repository.Object);

        var result = await handler.Handle(
            new DeleteProductRequest(Guid.NewGuid()),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
        _repository.Verify(x => x.Delete(It.IsAny<Produto>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenProductExists_DeletesAndCommits()
    {
        Produto produto = new ProdutoBuilder().Build();
        _repository.Setup(x => x.GetWithDetailsAsync(produto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);
        var handler = new DeleteProductCommandHandler(_unitOfWork.Object, _repository.Object);

        var result = await handler.Handle(
            new DeleteProductRequest(produto.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
        _repository.Verify(x => x.Delete(produto), Times.Once);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
