namespace MusicMasterShop.Application.Queries.GetOrder;

public sealed record GetOrderItemResponse(
    Guid ProdutoId,
    string Nome,
    decimal Preco,
    int Quantidade);
