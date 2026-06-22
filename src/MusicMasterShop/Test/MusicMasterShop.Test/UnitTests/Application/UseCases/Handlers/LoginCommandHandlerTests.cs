namespace MusicMasterShop.Test.UnitTests.Application.UseCases.Handlers;

public sealed class LoginCommandHandlerTests
{
    private readonly Mock<IUsuarioRepository> _repository = new();
    private readonly Mock<IPasswordHasher<Usuario>> _passwordHasher = new();
    private readonly Mock<IJwtTokenService> _jwtTokenService = new();

    [Fact]
    public async Task Handle_WithInvalidRequest_ReturnsBadRequest()
    {
        var result = await CreateHandler().Handle(
            new LoginRequestBuilder().WithEmail("invalid").Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsUnauthorized()
    {
        _repository.Setup(x => x.GetByEmailAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario)null!);

        var result = await CreateHandler().Handle(
            new LoginRequestBuilder().Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WhenPasswordIsInvalid_ReturnsUnauthorized()
    {
        Usuario usuario = new UsuarioBuilder().Build();
        _repository.Setup(x => x.GetByEmailAsync(usuario.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _passwordHasher.Setup(x => x.VerifyHashedPassword(usuario, usuario.Senha, "secret123"))
            .Returns(PasswordVerificationResult.Failed);

        var result = await CreateHandler().Handle(
            new LoginRequestBuilder().WithEmail(usuario.Email).Build(),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsToken()
    {
        Usuario usuario = new UsuarioBuilder().Build();
        DateTime expiresAt = DateTime.UtcNow.AddHours(1);
        _repository.Setup(x => x.GetByEmailAsync(usuario.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _passwordHasher.Setup(x => x.VerifyHashedPassword(usuario, usuario.Senha, "secret123"))
            .Returns(PasswordVerificationResult.Success);
        _jwtTokenService.Setup(x => x.GenerateToken(usuario))
            .Returns(new JwtTokenResult("jwt-token", expiresAt));

        var result = await CreateHandler().Handle(
            new LoginRequestBuilder().WithEmail(usuario.Email).Build(),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("jwt-token", result.Data!.Token);
        Assert.Equal(expiresAt, result.Data.ExpiraEm);
    }

    private LoginCommandHandler CreateHandler() =>
        new(_repository.Object, _passwordHasher.Object, _jwtTokenService.Object);
}
