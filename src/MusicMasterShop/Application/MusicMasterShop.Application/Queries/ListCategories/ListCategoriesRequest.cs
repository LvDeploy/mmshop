using MediatR;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Domain.Core.Pagination;

namespace MusicMasterShop.Application.Queries.ListCategories
{
    public sealed record ListCategoriesRequest()
    : IRequest<BaseResponse<IEnumerable<ListCategoriesResponse>>>
    {
    }
}
