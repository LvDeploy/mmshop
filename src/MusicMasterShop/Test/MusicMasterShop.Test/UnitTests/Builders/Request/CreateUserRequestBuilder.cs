namespace MusicMasterShop.Test.UnitTests.Builders.Request;

public sealed class CreateUserRequestBuilder
{
    private string _email = "seller@musicmaster.com";
    private string _nome = "Seller";
    private TipoUsuario? _tipoUsuario = TipoUsuario.Vendedor;
    private string _senha = "secret123";

    public CreateUserRequestBuilder WithEmail(string value)
    {
        _email = value;
        return this;
    }

    public CreateUserRequestBuilder WithNome(string value)
    {
        _nome = value;
        return this;
    }

    public CreateUserRequestBuilder WithTipoUsuario(TipoUsuario? value)
    {
        _tipoUsuario = value;
        return this;
    }

    public CreateUserRequestBuilder WithSenha(string value)
    {
        _senha = value;
        return this;
    }

    public CreateUserRequest Build() => new(_email, _nome, _tipoUsuario, _senha);
}
