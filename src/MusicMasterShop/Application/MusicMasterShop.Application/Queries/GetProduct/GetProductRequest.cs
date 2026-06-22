using MediatR;
using MusicMasterShop.Application.Abstractions.Response;

namespace MusicMasterShop.Application.Queries.GetProduct
{
    public record GetProductRequest(Guid Id) : IRequest<BaseResponse<GetProductResponse>>
    {
    }
}
 