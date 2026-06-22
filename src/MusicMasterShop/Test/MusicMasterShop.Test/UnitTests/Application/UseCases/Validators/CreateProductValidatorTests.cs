namespace MusicMasterShop.Test.UnitTests.Application.UseCases.Validators;

public sealed class CreateProductValidatorTests
{
    private readonly CreateProductValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        _validator.TestValidate(new CreateProductRequestBuilder().Build())
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithInvalidFields_HasExpectedErrors()
    {
        var request = new CreateProductRequestBuilder()
            .WithNome(string.Empty)
            .WithDescricao(string.Empty)
            .WithPreco(0)
            .WithTipoCategoria(null)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Nome);
        result.ShouldHaveValidationErrorFor(x => x.Descricao);
        result.ShouldHaveValidationErrorFor(x => x.Preco);
        result.ShouldHaveValidationErrorFor(x => x.TipoCategoriaId);
    }
}
