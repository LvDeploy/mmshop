namespace MusicMasterShop.Test.UnitTests.Application.UseCases.Handlers;

public sealed class CreateCartCommandHandlerTests
{
    private readonly Mock<IUserInfo> _userInfo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICarrinhoRepository> _carrinhoRepository = new();
    private readonly Mock<IProdutoRepository> _produtoRepository = new();
    private readonly Mock<IUsuarioRepository> _usuarioRepository = new();

    [Fact]
    public async Task Handle_WithInvalidRequest_ReturnsBadRequest()
    {
        var result = await CreateHandler().Handle(
            new CreateCartRequestBuilder().WithQuantidade(0).Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotSeller_ReturnsForbidden()
    {
        _userInfo.SetupGet(x => x.IsAuthenticated).Returns(true);
        _userInfo.SetupGet(x => x.TipoUsuario).Returns(TipoUsuario.Administrador);

        var result = await CreateHandler().Handle(
            new CreateCartRequestBuilder().Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Forbidden, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsNotFound()
    {
        SetupSeller(Guid.NewGuid());
        _produtoRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Produto)null!);

        var result = await CreateHandler().Handle(
            new CreateCartRequestBuilder().Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsNotFound()
    {
        Guid userId = Guid.NewGuid();
        Produto produto = new ProdutoBuilder().Build();
        SetupSeller(userId);
        _produtoRepository.Setup(x => x.Get(produto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);
        _usuarioRepository.Setup(x => x.Get(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario)null!);

        var result = await CreateHandler().Handle(
            new CreateCartRequestBuilder().WithProdutoId(produto.Id).Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenThereIsNoActiveCart_CreatesCartAndCommits()
    {
        Usuario usuario = new UsuarioBuilder().Build();
        Produto produto = new ProdutoBuilder().Build();
        SetupSeller(usuario.Id);
        _produtoRepository.Setup(x => x.Get(produto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);
        _usuarioRepository.Setup(x => x.Get(usuario.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _carrinhoRepository.Setup(x => x.GetActiveByUsuarioIdAsync(
                usuario.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Carrinho)null!);

        var result = await CreateHandler().Handle(
            new CreateCartRequestBuilder()
                .WithProdutoId(produto.Id)
                .WithQuantidade(3)
                .Build(),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        _carrinhoRepository.Verify(
            x => x.Create(It.Is<Carrinho>(
                cart => cart.UsuarioId == usuario.Id && cart.Produtos.Single().Quantidade == 3)),
            Times.Once);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenActiveCartExists_UpdatesCartAndCommits()
    {
        Usuario usuario = new UsuarioBuilder().Build();
        Produto produto = new ProdutoBuilder().Build();
        Carrinho carrinho = new CarrinhoBuilder()
            .WithUsuario(usuario)
            .WithProdutos((produto, 1))
            .Build();
        SetupSeller(usuario.Id);
        _produtoRepository.Setup(x => x.Get(produto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);
        _usuarioRepository.Setup(x => x.Get(usuario.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _carrinhoRepository.Setup(x => x.GetActiveByUsuarioIdAsync(
                usuario.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(carrinho);

        var result = await CreateHandler().Handle(
            new CreateCartRequestBuilder()
                .WithProdutoId(produto.Id)
                .WithQuantidade(4)
                .Build(),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(4, carrinho.Produtos.Single().Quantidade);
        _carrinhoRepository.Verify(x => x.Create(It.IsAny<Carrinho>()), Times.Never);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private void SetupSeller(Guid id)
    {
        _userInfo.SetupGet(x => x.IsAuthenticated).Returns(true);
        _userInfo.SetupGet(x => x.TipoUsuario).Returns(TipoUsuario.Vendedor);
        _userInfo.SetupGet(x => x.Id).Returns(id);
    }

    private CreateCartCommandHandler CreateHandler() =>
        new(
            _userInfo.Object,
            _unitOfWork.Object,
            _carrinhoRepository.Object,
            _produtoRepository.Object,
            _usuarioRepository.Object);
}
