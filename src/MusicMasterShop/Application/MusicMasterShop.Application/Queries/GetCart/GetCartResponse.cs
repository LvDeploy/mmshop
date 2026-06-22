namespace MusicMasterShop.Application.Queries.GetCart;

public sealed record GetCartResponse(
    Guid Id,
    DateTime DataCriacao,
    DateTime? DataAtualizacao,
    IReadOnlyCollection<GetCartItemResponse> Produtos);
