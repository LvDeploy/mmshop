namespace MusicMasterShop.Application.UseCases.UpdateCart;

public sealed record UpdateCartItemRequest(Guid ProdutoId, int Quantidade);
