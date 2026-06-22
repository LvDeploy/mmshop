namespace MusicMasterShop.Application.Queries.GetCart;

public sealed record GetCartItemResponse(
    Guid ProdutoId,
    string Nome,
    decimal Preco,
    uint QtdDisponivel,
    int Quantidade);
