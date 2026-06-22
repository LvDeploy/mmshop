namespace MusicMasterShop.Test.UnitTests.Application.UseCases.Validators;

public sealed class CreateOrderValidatorTests
{
    private readonly CreateOrderValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        _validator.TestValidate(new CreateOrderRequestBuilder().Build())
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithInvalidFields_HasExpectedErrors()
    {
        var request = new CreateOrderRequestBuilder()
            .WithCarrinhoId(Guid.Empty)
            .WithDocumentoCliente(string.Empty)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.CarrinhoId);
        result.ShouldHaveValidationErrorFor(x => x.DocumentoCliente);
    }
}
