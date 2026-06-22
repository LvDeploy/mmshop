using MediatR;
using MusicMasterShop.Application.Abstractions.Request;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Domain.Core.Pagination;

namespace MusicMasterShop.Application.Queries.ListProductsPaged;

public sealed record ListProductsPagedRequest(int PageNumber = 1, int PageSize = 10)
    : BaseRequest, IRequest<BaseResponse<PagedResult<ListProductsPagedResponse>>>
{
    public override bool IsValid()
    {
        ValidationResult = new ListProductsPagedValidator().Validate(this);
        return ValidationResult.IsValid;
    }
}
