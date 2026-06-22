namespace MusicMasterShop.Test.UnitTests.Application.UseCases.Validators;

public sealed class CreateCartValidatorTests
{
    private readonly CreateCartValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        var request = new CreateCartRequestBuilder().Build();

        _validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithInvalidFields_HasExpectedErrors()
    {
        var request = new CreateCartRequestBuilder()
            .WithQuantidade(0)
            .WithProdutoId(Guid.Empty)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Quantidade);
        result.ShouldHaveValidationErrorFor(x => x.ProdutoId);
    }
}
