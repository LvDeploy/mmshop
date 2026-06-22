namespace MusicMasterShop.Test.UnitTests.Application.Queries.Handlers;

public sealed class GetOrderQueryHandlerTests
{
    private readonly Mock<IUserInfo> _userInfo = new();
    private readonly Mock<IPedidoRepository> _repository = new();

    [Fact]
    public async Task Handle_WhenOrderDoesNotExist_ReturnsNotFound()
    {
        SetupSeller(Guid.NewGuid());
        _repository.Setup(x => x.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pedido)null!);
        var handler = new GetOrderQueryHandler(_userInfo.Object, _repository.Object);

        var result = await handler.Handle(
            new GetOrderRequest(Guid.NewGuid()),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenOrderBelongsToAnotherUser_ReturnsForbidden()
    {
        Pedido pedido = new PedidoBuilder().Build();
        SetupSeller(Guid.NewGuid());
        _repository.Setup(x => x.GetWithDetailsAsync(pedido.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedido);
        var handler = new GetOrderQueryHandler(_userInfo.Object, _repository.Object);

        var result = await handler.Handle(
            new GetOrderRequest(pedido.Id),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Forbidden, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenOrderExists_ReturnsMappedOrder()
    {
        Usuario usuario = new UsuarioBuilder().Build();
        Carrinho carrinho = new CarrinhoBuilder().WithUsuario(usuario).Build();
        Pedido pedido = new PedidoBuilder().WithCarrinho(carrinho).Build();
        SetupSeller(usuario.Id);
        _repository.Setup(x => x.GetWithDetailsAsync(pedido.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedido);
        var handler = new GetOrderQueryHandler(_userInfo.Object, _repository.Object);

        var result = await handler.Handle(
            new GetOrderRequest(pedido.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(pedido.Id, result.Data!.Id);
        Assert.Equal(carrinho.Produtos.Count, result.Data.Produtos.Count);
    }

    private void SetupSeller(Guid userId)
    {
        _userInfo.SetupGet(x => x.IsAuthenticated).Returns(true);
        _userInfo.SetupGet(x => x.TipoUsuario).Returns(TipoUsuario.Vendedor);
        _userInfo.SetupGet(x => x.Id).Returns(userId);
    }
}
