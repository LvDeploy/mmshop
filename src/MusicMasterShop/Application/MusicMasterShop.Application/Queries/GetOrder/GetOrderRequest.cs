using MediatR;
using MusicMasterShop.Application.Abstractions.Response;

namespace MusicMasterShop.Application.Queries.GetOrder;

public sealed record GetOrderRequest(Guid PedidoId)
    : IRequest<BaseResponse<GetOrderResponse>>;
