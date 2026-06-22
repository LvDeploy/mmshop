namespace MusicMasterShop.Test.UnitTests.Builders.Domain;

public sealed class UsuarioBuilder
{
    private string _email = "seller@musicmaster.com";
    private string _nome = "Seller";
    private string _senha = "hashed-password";
    private TipoUsuario _tipo = TipoUsuario.Vendedor;

    public UsuarioBuilder WithNome(string value)
    {
        _nome = value;
        return this;
    }

    public UsuarioBuilder WithTipo(TipoUsuario value)
    {
        _tipo = value;
        return this;
    }

    public UsuarioBuilder WithSenha(string value)
    {
        _senha = value;
        return this;
    }

    public Usuario Build() => Usuario.Create(_email, _nome, _senha, _tipo);
}
