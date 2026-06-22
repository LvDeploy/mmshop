namespace MusicMasterShop.Test.UnitTests.Application.UseCases.Handlers;

public sealed class CreateOrderCommandHandlerTests
{
    private readonly Mock<IUserInfo> _userInfo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICarrinhoRepository> _carrinhoRepository = new();
    private readonly Mock<IPedidoRepository> _pedidoRepository = new();

    [Fact]
    public async Task Handle_WithInvalidRequest_ReturnsBadRequest()
    {
        var result = await CreateHandler().Handle(
            new CreateOrderRequestBuilder().WithCarrinhoId(Guid.Empty).Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotSeller_ReturnsForbidden()
    {
        _userInfo.SetupGet(x => x.TipoUsuario).Returns(TipoUsuario.Administrador);

        var result = await CreateHandler().Handle(
            new CreateOrderRequestBuilder().Build(),
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
            new CreateOrderRequestBuilder().Build(),
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
            new CreateOrderRequestBuilder().WithCarrinhoId(carrinho.Id).Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Forbidden, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenCartIsInactive_ReturnsBadRequest()
    {
        Usuario usuario = new UsuarioBuilder().Build();
        Carrinho carrinho = new CarrinhoBuilder().WithUsuario(usuario).Inativo().Build();
        SetupCart(usuario, carrinho);

        var result = await CreateHandler().Handle(
            new CreateOrderRequestBuilder().WithCarrinhoId(carrinho.Id).Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenCartIsEmpty_ReturnsBadRequest()
    {
        Usuario usuario = new UsuarioBuilder().Build();
        Carrinho carrinho = new CarrinhoBuilder()
            .WithUsuario(usuario)
            .WithProdutos()
            .Build();
        SetupCart(usuario, carrinho);

        var result = await CreateHandler().Handle(
            new CreateOrderRequestBuilder().WithCarrinhoId(carrinho.Id).Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenProductStockIsInsufficient_ReturnsBadRequest()
    {
        Usuario usuario = new UsuarioBuilder().Build();
        Produto produto = new ProdutoBuilder().WithQuantidade(1).Build();
        Carrinho carrinho = new CarrinhoBuilder()
            .WithUsuario(usuario)
            .WithProdutos((produto, 2))
            .Build();
        SetupCart(usuario, carrinho);

        var result = await CreateHandler().Handle(
            new CreateOrderRequestBuilder().WithCarrinhoId(carrinho.Id).Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WithValidCart_CreatesOrderUpdatesStockAndCommits()
    {
        Usuario usuario = new UsuarioBuilder().Build();
        Produto produto = new ProdutoBuilder().WithQuantidade(10).Build();
        Carrinho carrinho = new CarrinhoBuilder()
            .WithUsuario(usuario)
            .WithProdutos((produto, 3))
            .Build();
        SetupCart(usuario, carrinho);

        var result = await CreateHandler().Handle(
            new CreateOrderRequestBuilder().WithCarrinhoId(carrinho.Id).Build(),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(carrinho.Ativo);
        Assert.Equal((uint)7, produto.QtdDisponivel);
        _pedidoRepository.Verify(
            x => x.Create(It.Is<Pedido>(
                order => order.CarrinhoId == carrinho.Id
                    && order.Status == StatusPedido.Validado)),
            Times.Once);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private void SetupCart(Usuario usuario, Carrinho carrinho)
    {
        SetupSeller(usuario.Id);
        _carrinhoRepository.Setup(x => x.GetWithProductsAsync(
                carrinho.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(carrinho);
    }

    private void SetupSeller(Guid id)
    {
        _userInfo.SetupGet(x => x.IsAuthenticated).Returns(true);
        _userInfo.SetupGet(x => x.TipoUsuario).Returns(TipoUsuario.Vendedor);
        _userInfo.SetupGet(x => x.Id).Returns(id);
    }

    private CreateOrderCommandHandler CreateHandler() =>
        new(
            _userInfo.Object,
            _unitOfWork.Object,
            _carrinhoRepository.Object,
            _pedidoRepository.Object);
}
