namespace MusicMasterShop.Test.UnitTests.Builders.Domain;

public sealed class CategoriaBuilder
{
    private TipoCategoria _tipo = TipoCategoria.Corda;
    private string _nome = "Cordas";

    public CategoriaBuilder WithTipo(TipoCategoria value)
    {
        _tipo = value;
        return this;
    }

    public CategoriaBuilder WithNome(string value)
    {
        _nome = value;
        return this;
    }

    public Categoria Build() => Categoria.Create(_tipo, _nome);
}
