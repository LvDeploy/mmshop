namespace MusicMasterShop.Test.UnitTests.Application.UseCases.Handlers;

public sealed class CreateUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IUsuarioRepository> _repository = new();
    private readonly Mock<IPasswordHasher<Usuario>> _passwordHasher = new();

    [Fact]
    public async Task Handle_WithInvalidRequest_ReturnsBadRequest()
    {
        var handler = CreateHandler();

        var result = await handler.Handle(
            new CreateUserRequestBuilder().WithEmail("invalid").Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
        _repository.Verify(x => x.Create(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithValidRequest_HashesPasswordCreatesUserAndCommits()
    {
        const string hash = "generated-hash";
        _passwordHasher.Setup(x => x.HashPassword(It.IsAny<Usuario>(), "secret123"))
            .Returns(hash);
        var handler = CreateHandler();

        var result = await handler.Handle(
            new CreateUserRequestBuilder().WithEmail("  seller@musicmaster.com ").Build(),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        _repository.Verify(
            x => x.Create(It.Is<Usuario>(
                user => user.Email == "seller@musicmaster.com" && user.Senha == hash)),
            Times.Once);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private CreateUserCommandHandler CreateHandler() =>
        new(_unitOfWork.Object, _repository.Object, _passwordHasher.Object);
}
