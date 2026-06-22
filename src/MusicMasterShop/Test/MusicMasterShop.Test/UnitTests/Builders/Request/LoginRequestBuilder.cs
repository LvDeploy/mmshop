namespace MusicMasterShop.Test.UnitTests.Builders.Request;

public sealed class LoginRequestBuilder
{
    private string _email = "seller@musicmaster.com";
    private string _senha = "secret123";

    public LoginRequestBuilder WithEmail(string value)
    {
        _email = value;
        return this;
    }

    public LoginRequestBuilder WithSenha(string value)
    {
        _senha = value;
        return this;
    }

    public LoginRequest Build() => new(_email, _senha);
}
