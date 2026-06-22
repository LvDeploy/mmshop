namespace MusicMasterShop.Test.UnitTests.Application.Queries.Handlers;

public sealed class GetUserQueryHandlerTests
{
    private readonly Mock<IUsuarioRepository> _repository = new();

    [Fact]
    public async Task Handle_WhenUserExists_ReturnsMappedUser()
    {
        Usuario usuario = new UsuarioBuilder().Build();
        _repository.Setup(x => x.Get(usuario.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        var handler = new GetUserQueryHandler(_repository.Object);

        var result = await handler.Handle(
            new GetUserRequest(usuario.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(usuario.Id, result.Data!.Id);
        Assert.Equal(usuario.Tipo, result.Data.TipoUsuario);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsNotFound()
    {
        _repository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario)null!);
        var handler = new GetUserQueryHandler(_repository.Object);

        var result = await handler.Handle(
            new GetUserRequest(Guid.NewGuid()),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }
}
