namespace MusicMasterShop.Test.UnitTests.Application.UseCases.Validators;

public sealed class UpdateProductValidatorTests
{
    private readonly UpdateProductValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        _validator.TestValidate(new UpdateProductRequestBuilder().Build())
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithInvalidFields_HasExpectedErrors()
    {
        var request = new UpdateProductRequestBuilder()
            .WithId(Guid.Empty)
            .WithNome(string.Empty)
            .WithDescricao(string.Empty)
            .WithTipoCategoria((TipoCategoria)999)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.Nome);
        result.ShouldHaveValidationErrorFor(x => x.Descricao);
        result.ShouldHaveValidationErrorFor(x => x.TipoCategoriaId);
    }
}
