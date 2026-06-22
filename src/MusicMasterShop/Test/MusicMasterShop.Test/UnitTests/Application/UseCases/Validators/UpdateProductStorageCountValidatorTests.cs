namespace MusicMasterShop.Test.UnitTests.Application.UseCases.Validators;

public sealed class UpdateProductStorageCountValidatorTests
{
    private readonly UpdateProductStorageCountValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        _validator.TestValidate(new UpdateProductStorageCountRequestBuilder().Build())
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithInvalidFields_HasExpectedErrors()
    {
        var request = new UpdateProductStorageCountRequestBuilder()
            .WithId(Guid.Empty)
            .WithQuantidade(-1)
            .WithNumeroNotaFiscal(string.Empty)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.Quantidade);
        result.ShouldHaveValidationErrorFor(x => x.NumeroNotaFiscal);
    }
}
