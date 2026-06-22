namespace MusicMasterShop.Test.UnitTests.Application.UseCases.Validators;

public sealed class LoginValidatorTests
{
    private readonly LoginValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        _validator.TestValidate(new LoginRequestBuilder().Build())
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithInvalidFields_HasExpectedErrors()
    {
        var request = new LoginRequestBuilder()
            .WithEmail("invalid")
            .WithSenha(string.Empty)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.Senha);
    }
}
