namespace MusicMasterShop.Test.UnitTests.Application.UseCases.Validators;

public sealed class CreateUserValidatorTests
{
    private readonly CreateUserValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        _validator.TestValidate(new CreateUserRequestBuilder().Build())
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithInvalidFields_HasExpectedErrors()
    {
        var request = new CreateUserRequestBuilder()
            .WithEmail("invalid")
            .WithNome(string.Empty)
            .WithSenha("123")
            .WithTipoUsuario(null)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.Nome);
        result.ShouldHaveValidationErrorFor(x => x.Senha);
        result.ShouldHaveValidationErrorFor(x => x.TipoUsuario);
    }
}
