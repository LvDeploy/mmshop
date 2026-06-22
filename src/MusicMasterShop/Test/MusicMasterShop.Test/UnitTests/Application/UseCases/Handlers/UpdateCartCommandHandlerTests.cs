namespace MusicMasterShop.Test.UnitTests.Application.UseCases.Handlers;

public sealed class UpdateCartCommandHandlerTests
{
    private readonly Mock<IUserInfo> _userInfo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICarrinhoRepository> _carrinhoRepository = new();
    private readonly Mock<IProdutoRepository> _produtoRepository = new();

    [Fact]
    public async Task Handle_WithInvalidRequest_ReturnsBadRequest()
    {
        var result = await CreateHandler().Handle(
            new UpdateCartRequestBuilder().WithCarrinhoId(Guid.Empty).Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotSeller_ReturnsForbidden()
    {
        _userInfo.SetupGet(x => x.TipoUsuario).Returns(TipoUsuario.Administrador);

        var result = await CreateHandler().Handle(
            new UpdateCartRequestBuilder().Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Forbidden, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenCartDoesNotExist_ReturnsNotFound()
    {
        SetupSeller(Guid.NewGuid());
        _carrinhoRepository.Setup(x => x.GetWithProductsAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Carrinho)null!);

        var result = await CreateHandler().Handle(
            new UpdateCartRequestBuilder().Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenCartBelongsToAnotherUser_ReturnsForbidden()
    {
        Carrinho carrinho = new CarrinhoBuilder().Build();
        SetupSeller(Guid.NewGuid());
        _carrinhoRepository.Setup(x => x.GetWithProductsAsync(
                carrinho.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(carrinho);

        var result = await CreateHandler().Handle(
            new UpdateCartRequestBuilder().WithCarrinhoId(carrinho.Id).Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Forbidden, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenCartIsInactive_ReturnsBadRequest()
    {
        Usuario usuario = new UsuarioBuilder().Build();
        Carrinho carrinho = new CarrinhoBuilder().WithUsuario(usuario).Inativo().Build();
        SetupSeller(usuario.Id);
        _carrinhoRepository.Setup(x => x.GetWithProductsAsync(
                carrinho.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(carrinho);

        var result = await CreateHandler().Handle(
            new UpdateCartRequestBuilder().WithCarrinhoId(carrinho.Id).Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsNotFound()
    {
        Usuario usuario = new UsuarioBuilder().Build();
        Carrinho carrinho = new CarrinhoBuilder().WithUsuario(usuario).Build();
        Guid missingProductId = Guid.NewGuid();
        SetupSeller(usuario.Id);
        _carrinhoRepository.Setup(x => x.GetWithProductsAsync(
                carrinho.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(carrinho);
        _produtoRepository.Setup(x => x.Get(missingProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Produto)null!);

        var result = await CreateHandler().Handle(
            new UpdateCartRequestBuilder()
                .WithCarrinhoId(carrinho.Id)
                .WithProdutos([new UpdateCartItemRequest(missingProductId, 2)])
                .Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WithValidRequest_UpdatesItemsAndCommits()
    {
        Usuario usuario = new UsuarioBuilder().Build();
        Produto removedProduct = new ProdutoBuilder().Build();
        Produto addedProduct = new ProdutoBuilder().WithNome("Baixo").Build();
        Carrinho carrinho = new CarrinhoBuilder()
            .WithUsuario(usuario)
            .WithProdutos((removedProduct, 1))
            .Build();
        SetupSeller(usuario.Id);
        _carrinhoRepository.Setup(x => x.GetWithProductsAsync(
                carrinho.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(carrinho);
        _produtoRepository.Setup(x => x.Get(addedProduct.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(addedProduct);
        var request = new UpdateCartRequestBuilder()
            .WithCarrinhoId(carrinho.Id)
            .WithProdutos(
            [
                new UpdateCartItemRequest(removedProduct.Id, 0),
                new UpdateCartItemRequest(addedProduct.Id, 3)
            ])
            .Build();

        var result = await CreateHandler().Handle(request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(carrinho.Produtos);
        Assert.Equal(addedProduct.Id, carrinho.Produtos.Single().Produto.Id);
        Assert.Equal(3, carrinho.Produtos.Single().Quantidade);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private void SetupSeller(Guid id)
    {
        _userInfo.SetupGet(x => x.IsAuthenticated).Returns(true);
        _userInfo.SetupGet(x => x.TipoUsuario).Returns(TipoUsuario.Vendedor);
        _userInfo.SetupGet(x => x.Id).Returns(id);
    }

    private UpdateCartCommandHandler CreateHandler() =>
        new(
            _userInfo.Object,
            _unitOfWork.Object,
            _carrinhoRepository.Object,
            _produtoRepository.Object);
}
