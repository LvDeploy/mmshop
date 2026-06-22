namespace MusicMasterShop.Test.UnitTests.Application.UseCases.Handlers;

public sealed class CreateProductCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICategoriaRepository> _categoriaRepository = new();
    private readonly Mock<IProdutoRepository> _produtoRepository = new();

    [Fact]
    public async Task Handle_WithInvalidRequest_ReturnsBadRequest()
    {
        var request = new CreateProductRequestBuilder().WithNome(string.Empty).Build();
        var handler = CreateHandler();

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
        _produtoRepository.Verify(x => x.Create(It.IsAny<Produto>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ReturnsUnauthorized()
    {
        _categoriaRepository.Setup(x => x.GetByTipoAsync(
                It.IsAny<TipoCategoria>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Categoria)null!);
        var handler = CreateHandler();

        var result = await handler.Handle(
            new CreateProductRequestBuilder().Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WithValidRequest_CreatesProductAndCommits()
    {
        Categoria categoria = new CategoriaBuilder().Build();
        _categoriaRepository.Setup(x => x.GetByTipoAsync(
                categoria.Tipo,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoria);
        var handler = CreateHandler();

        var result = await handler.Handle(
            new CreateProductRequestBuilder().WithTipoCategoria(categoria.Tipo).Build(),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        _produtoRepository.Verify(
            x => x.Create(It.Is<Produto>(p => p.CategoriaId == categoria.Id)),
            Times.Once);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private CreateProductCommandHandler CreateHandler() =>
        new(_unitOfWork.Object, _categoriaRepository.Object, _produtoRepository.Object);
}
