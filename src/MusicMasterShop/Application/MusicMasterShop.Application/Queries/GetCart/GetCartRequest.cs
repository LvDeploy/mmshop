using MediatR;
using MusicMasterShop.Application.Abstractions.Response;

namespace MusicMasterShop.Application.Queries.GetCart;

public sealed record GetCartRequest : IRequest<BaseResponse<GetCartResponse>>;
