namespace MusicMasterShop.Test.UnitTests.Application.Queries.Handlers;

public sealed class GetCartQueryHandlerTests
{
    private readonly Mock<IUserInfo> _userInfo = new();
    private readonly Mock<ICarrinhoRepository> _repository = new();

    [Fact]
    public async Task Handle_WhenUserIsNotSeller_ReturnsForbidden()
    {
        _userInfo.SetupGet(x => x.IsAuthenticated).Returns(true);
        _userInfo.SetupGet(x => x.TipoUsuario).Returns(TipoUsuario.Administrador);
        var handler = new GetCartQueryHandler(_userInfo.Object, _repository.Object);

        var result = await handler.Handle(new GetCartRequest(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Forbidden, result.ErrorType);
        _repository.Verify(
            x => x.GetActiveByUsuarioIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenActiveCartDoesNotExist_ReturnsNotFound()
    {
        Guid userId = Guid.NewGuid();
        SetupSeller(userId);
        _repository.Setup(x => x.GetActiveByUsuarioIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Carrinho)null!);
        var handler = new GetCartQueryHandler(_userInfo.Object, _repository.Object);

        var result = await handler.Handle(new GetCartRequest(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenActiveCartExists_ReturnsMappedCart()
    {
        Usuario usuario = new UsuarioBuilder().Build();
        Carrinho carrinho = new CarrinhoBuilder().WithUsuario(usuario).Build();
        SetupSeller(usuario.Id);
        _repository.Setup(x => x.GetActiveByUsuarioIdAsync(usuario.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(carrinho);
        var handler = new GetCartQueryHandler(_userInfo.Object, _repository.Object);

        var result = await handler.Handle(new GetCartRequest(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(carrinho.Id, result.Data!.Id);
        Assert.Equal(carrinho.Produtos.Count, result.Data.Produtos.Count);
    }

    private void SetupSeller(Guid userId)
    {
        _userInfo.SetupGet(x => x.IsAuthenticated).Returns(true);
        _userInfo.SetupGet(x => x.TipoUsuario).Returns(TipoUsuario.Vendedor);
        _userInfo.SetupGet(x => x.Id).Returns(userId);
    }
}
