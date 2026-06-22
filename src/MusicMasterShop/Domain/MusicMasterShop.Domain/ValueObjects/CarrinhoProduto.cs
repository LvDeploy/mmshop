using MusicMasterShop.Domain.Entities;

namespace MusicMasterShop.Domain.ValueObjects;

public sealed class CarrinhoProduto
{
    private CarrinhoProduto()
    {
    }

    private CarrinhoProduto(int quantidade, Produto produto)
    {
        Quantidade = quantidade;
        Produto = produto;
    }

    public int Quantidade { get; private set; }
    public Produto Produto { get; private set; } = null!;

    public static CarrinhoProduto Create(int quantidade, Produto produto)
    {
        return new CarrinhoProduto(quantidade, produto);
    }

    public void UpdateQuantidade(int quantidade)
    {
        Quantidade = quantidade;
    }
}
