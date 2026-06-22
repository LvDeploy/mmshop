namespace MusicMasterShop.Test.UnitTests.Application.Queries.Handlers;

public sealed class ListCategoriesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsMappedCategories()
    {
        var repository = new Mock<ICategoriaRepository>();
        List<Categoria> categorias =
        [
            new CategoriaBuilder().WithTipo(TipoCategoria.Corda).Build(),
            new CategoriaBuilder().WithTipo(TipoCategoria.Sopro).Build()
        ];
        repository.Setup(x => x.GetAll(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categorias);
        var handler = new ListCategoriesQueryHandler(repository.Object);

        var result = await handler.Handle(
            new ListCategoriesRequest(),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data!.Count());
    }
}
