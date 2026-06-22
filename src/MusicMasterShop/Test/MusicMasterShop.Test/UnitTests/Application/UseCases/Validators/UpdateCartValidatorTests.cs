namespace MusicMasterShop.Test.UnitTests.Application.UseCases.Validators;

public sealed class UpdateCartValidatorTests
{
    private readonly UpdateCartValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        _validator.TestValidate(new UpdateCartRequestBuilder().Build())
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithInvalidFields_HasExpectedErrors()
    {
        var request = new UpdateCartRequestBuilder()
            .WithCarrinhoId(Guid.Empty)
            .WithProdutos([new UpdateCartItemRequest(Guid.Empty, -1)])
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.CarrinhoId);
        result.ShouldHaveValidationErrorFor("Produtos[0].ProdutoId");
        result.ShouldHaveValidationErrorFor("Produtos[0].Quantidade");
    }
}
