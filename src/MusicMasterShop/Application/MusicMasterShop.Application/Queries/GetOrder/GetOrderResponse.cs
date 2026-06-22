using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.Queries.GetOrder;

public sealed record GetOrderResponse(
    Guid Id,
    Guid CarrinhoId,
    string NomeUsuario,
    string DocumentoCliente,
    StatusPedido Status,
    DateTime DataCriacao,
    DateTime? DataAtualizacao,
    IReadOnlyCollection<GetOrderItemResponse> Produtos);
