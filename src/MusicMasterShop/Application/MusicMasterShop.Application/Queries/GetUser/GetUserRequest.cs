using MediatR;
using MusicMasterShop.Application.Abstractions.Response;

namespace MusicMasterShop.Application.Queries.GetUser
{
    public record GetUserRequest(Guid id) : IRequest<BaseResponse<GetUserResponse>>
    {
    }
}
