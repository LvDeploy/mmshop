namespace MusicMasterShop.Test.UnitTests.Application.UseCases.Handlers;

public sealed class UpdateProductCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IProdutoRepository> _produtoRepository = new();
    private readonly Mock<ICategoriaRepository> _categoriaRepository = new();

    [Fact]
    public async Task Handle_WithInvalidRequest_ReturnsBadRequest()
    {
        var result = await CreateHandler().Handle(
            new UpdateProductRequestBuilder().WithId(Guid.Empty).Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsNotFound()
    {
        _produtoRepository.Setup(x => x.GetWithDetailsAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Produto)null!);

        var result = await CreateHandler().Handle(
            new UpdateProductRequestBuilder().Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ReturnsNotFound()
    {
        Produto produto = new ProdutoBuilder().Build();
        _produtoRepository.Setup(x => x.GetWithDetailsAsync(produto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);
        _categoriaRepository.Setup(x => x.GetByTipoAsync(
                It.IsAny<TipoCategoria>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Categoria)null!);

        var result = await CreateHandler().Handle(
            new UpdateProductRequestBuilder().WithId(produto.Id).Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WithValidRequest_UpdatesProductAndCommits()
    {
        Produto produto = new ProdutoBuilder().Build();
        Categoria categoria = new CategoriaBuilder().WithTipo(TipoCategoria.Sopro).Build();
        _produtoRepository.Setup(x => x.GetWithDetailsAsync(produto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);
        _categoriaRepository.Setup(x => x.GetByTipoAsync(
                categoria.Tipo,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoria);

        var result = await CreateHandler().Handle(
            new UpdateProductRequestBuilder()
                .WithId(produto.Id)
                .WithNome("Saxofone")
                .WithTipoCategoria(categoria.Tipo)
                .Build(),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Saxofone", produto.Nome);
        Assert.Equal(categoria.Id, produto.CategoriaId);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private UpdateProductCommandHandler CreateHandler() =>
        new(_unitOfWork.Object, _produtoRepository.Object, _categoriaRepository.Object);
}
